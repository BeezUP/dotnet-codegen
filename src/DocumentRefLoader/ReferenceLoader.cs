using DocumentRefLoader.Settings;
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
        internal readonly Dictionary<Uri, ReferenceLoader> _otherLoaders;

        private readonly IReferenceLoaderSettings _settings;
        internal readonly Uri _documentUri;
        internal readonly Uri _documentFolder;

        public ReferenceLoader(string fileUri, ReferenceLoaderStrategy strategy)
            : this(fileUri.GetAbsoluteUri(), null, strategy)
        { }

        private ReferenceLoader(Uri documentUri, Dictionary<Uri, ReferenceLoader> otherLoaders, ReferenceLoaderStrategy strategy, string authorization = null)
            : this(documentUri, otherLoaders, strategy.GetSettings(), authorization)
        { }

        private ReferenceLoader(Uri documentUri, Dictionary<Uri, ReferenceLoader> otherLoaders, IReferenceLoaderSettings settings, string authorization = null)
        {
            _documentUri = documentUri.GetAbsolute();
            _documentFolder = documentUri.GetFolder();

            _otherLoaders = otherLoaders ?? new Dictionary<Uri, ReferenceLoader>() { { _documentUri, this } };
            _settings = settings;
        }

        internal string _originalJson;
        private JObject _rootJObj;
        private async Task EnsureJsonLoadedAsync()
        {
            if (_rootJObj != null) return;
            var originalDocument = await _documentUri.DownloadDocumentAsync();
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



        public async Task<string> GetRefResolvedYamlAsync() => _settings.YamlSerialize(await GetRefResolvedJObjectAsync());
        public async Task<string> GetRefResolvedJsonAsync() => _settings.JsonSerialize(await GetRefResolvedJObjectAsync());

        public async Task<JObject> GetRefResolvedJObjectAsync()
        {
            await EnsureRefResolvedAsync();
            return _rootJObj;
        }

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

            if (_otherLoaders.TryGetValue(refInfo.AbsoluteDocumentUri, out var loader))
                return loader;

            loader = new ReferenceLoader(refInfo.AbsoluteDocumentUri, _otherLoaders, _settings);
            _otherLoaders[refInfo.AbsoluteDocumentUri] = loader;
            return loader;
        }
    }
}
