﻿using DocumentRefLoader.Settings;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentRefLoader
{
    /// <summary>
    /// https://swagger.io/docs/specification/using-ref/
    /// </summary>
    public sealed class ReferenceLoader
    {
        internal readonly Dictionary<Uri, ReferenceLoader> _loaders;

        private readonly IReferenceLoaderSettings _settings;
        private readonly string _authorization;
        internal readonly Uri _documentUri;
        internal readonly Uri _documentFolder;

        public ReferenceLoader(string fileUri, ReferenceLoaderStrategy strategy, string authorization = null)
            : this(fileUri.GetAbsoluteUri(), null, strategy, authorization)
        { }

        internal ReferenceLoader(string fileUri, Dictionary<Uri, ReferenceLoader> loaders, ReferenceLoaderStrategy strategy, string authorization = null)
            : this(fileUri.GetAbsoluteUri(), loaders, strategy.GetSettings(), authorization)
        { }

        internal ReferenceLoader(Uri documentUri, Dictionary<Uri, ReferenceLoader> loaders, ReferenceLoaderStrategy strategy, string authorization = null)
            : this(documentUri, loaders, strategy.GetSettings(), authorization)
        { }

        private ReferenceLoader(Uri documentUri, Dictionary<Uri, ReferenceLoader> loaders, IReferenceLoaderSettings settings, string authorization = null)
        {
            _documentUri = documentUri.GetAbsolute();
            _documentFolder = documentUri.GetFolder();

            _loaders = loaders ?? new Dictionary<Uri, ReferenceLoader>();
            _loaders[_documentUri] = this;

            _settings = settings;
            _authorization = authorization;
        }

        internal string _originalJson;
        private JObject _rootJObj;
        private async Task EnsureJsonLoadedAsync()
        {
            if (_rootJObj != null) return;
            var originalDocument = await _documentUri.DownloadDocumentAsync(_authorization);
            _rootJObj = _settings.Deserialise(originalDocument, _documentUri.ToString());
            _originalJson = _settings.JsonSerialize(_rootJObj);
        }

        bool _isResolved = false;
        internal string _finalJson;
        private async Task EnsureRefResolvedAsync()
        {
            if (_isResolved)
                return;

            await EnsureJsonLoadedAsync();

            _isResolved = true;
            await ResolveRefAsync(_rootJObj);
            _finalJson = _settings.JsonSerialize(_rootJObj);
        }

        public async Task<JObject> GetRefResolvedJObjectAsync()
        {
            await EnsureRefResolvedAsync();
            return _rootJObj;
        }

        public async Task<string> GetRefResolvedYamlAsync() => _settings.YamlSerialize(await GetRefResolvedJObjectAsync());
        public async Task<string> GetRefResolvedJsonAsync() => _settings.JsonSerialize(await GetRefResolvedJObjectAsync());


        private async Task ResolveRefAsync(JToken token)
        {
            if (!(token is JContainer container))
                return;

            var refProps = container.Descendants()
                .OfType<JProperty>()
                .Where(p => p.Name == Constants.REF_KEYWORD)
                .ToList();

            foreach (var refProperty in refProps)
            {
                var refPath = refProperty.Value.ToString();
                var refInfo = RefInfo.GetRefInfo(_documentUri, refPath);

                if (!_settings.ShouldResolveReference(refInfo)) continue;

                var replacement = await GetRefJTokenAsync(refInfo);
                await ResolveRefAsync(replacement);

                if (refProperty.Parent?.Parent == null)
                    // When a property has already been replaced by recursions, it has no more Parent
                    continue;

                // clone to avoid destroying the original documents
                replacement = replacement.DeepClone();
                if (replacement is JObject jobject)
                {
                    jobject[Constants.FROM_REF_PROPERTY_NAME] = refPath;
                    jobject[Constants.REF_NAME_PROPERTY_NAME] = refInfo.RefFriendlyName;
                }

                _settings.ApplyRefReplacement(refInfo, _rootJObj, refProperty, replacement, refInfo.AbsoluteDocumentUri);
            }
        }

        private async Task<JToken> GetRefJTokenAsync(RefInfo refInfo)
        {
            var loader = GetReferenceLoader(refInfo);
            var replacement = await loader.GetDocumentPartAsync(refInfo.InDocumentPath);
            return replacement;
        }

        private async Task<JToken> GetDocumentPartAsync(string path)
        {
            await EnsureRefResolvedAsync();

            var parts = path.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            JToken token = _rootJObj;
            foreach (var part in parts)
            {
                if (!string.IsNullOrWhiteSpace(part))
                {
                    try
                    {
                        token = token[part];
                    }
                    catch (Exception)
                    {
                        throw new InvalidDataException($"Unable to find path '{path}' in the document '{_documentUri}'.");
                    }
                }
            }

            return token;
        }

        private ReferenceLoader GetReferenceLoader(RefInfo refInfo)
        {
            if (refInfo.IsNestedInThisDocument)
                return this;

            if (_loaders.TryGetValue(refInfo.AbsoluteDocumentUri, out var loader))
                return loader;

            loader = new ReferenceLoader(refInfo.AbsoluteDocumentUri, _loaders, _settings);
            return loader;
        }
    }
}
