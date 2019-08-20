using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using DocumentRefLoader.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;

namespace DocumentRefLoader
{
    /// <summary>
    /// https://swagger.io/docs/specification/using-ref/
    /// </summary>
    public class ReferenceLoader
    {
        public const string REF_KEYWORD = "$ref";
        private readonly Dictionary<Uri, ReferenceLoader> _otherLoaders;
        private readonly IReferenceLoaderSettings _settings;
        private readonly Uri _documentUri;
        private readonly string _originalDocument;
        private readonly Uri _documentFolder;
        private readonly JObject _rootJObj;

        private static readonly Deserializer s_yamlDeserializer = new Deserializer();

        public ReferenceLoader(string fileUri, ReferenceLoaderStrategy strategy)
            : this(new Uri(fileUri, UriKind.RelativeOrAbsolute), null, strategy)
        { }

        private ReferenceLoader(Uri documentUri, Dictionary<Uri, ReferenceLoader> otherLoaders, ReferenceLoaderStrategy strategy)
            : this(documentUri, otherLoaders, strategy.GetSettings())
        { }

        private ReferenceLoader(Uri documentUri, Dictionary<Uri, ReferenceLoader> otherLoaders, IReferenceLoaderSettings settings)
        {
            _documentUri = documentUri;
            if (!_documentUri.IsAbsoluteUri)
                _documentUri = new Uri(new Uri(Path.Combine(Directory.GetCurrentDirectory(), ".")), _documentUri);
            _documentFolder = new Uri(_documentUri, ".");

            _otherLoaders = otherLoaders ?? new Dictionary<Uri, ReferenceLoader>() { { _documentUri, this } };
            _settings = settings;

            using (var webClient = new WebClient())
            {
                _originalDocument = webClient.DownloadString(_documentUri);
            }

            object yamlObject;
            using (var sr = new StringReader(_originalDocument))
            {
                yamlObject = s_yamlDeserializer.Deserialize(sr);
            }
            var json = JsonConvert.SerializeObject(yamlObject);
            _rootJObj = JObject.Parse(json);
        }


        public string GetRefResolvedYaml() => _settings.YamlSerialize(GetRefResolvedJObject());
        public string GetRefResolvedJson() => _settings.JsonSerialize(GetRefResolvedJObject());

        public JObject GetRefResolvedJObject()
        {
            EnsureRefResolved();
            return _rootJObj;
        }

        bool _isResolved = false;
        private void EnsureRefResolved()
        {
            if (_isResolved)
                return;

            _isResolved = true;
            ResolveRef(_rootJObj);
        }

        private void ResolveRef(JToken token)
        {
            if (!(token is JContainer container))
                return;

            var refProps = container.Descendants()
                .OfType<JProperty>()
                .Where(p => p.Name == REF_KEYWORD)
                .ToList();

            foreach (var refProperty in refProps)
            {
                var refPath = refProperty.Value.ToString();
                var refInfo = GetRefInfo(refPath);

                if (!_settings.ShouldResolveReference(refInfo)) continue;

                var replacement = GetRefJToken(refInfo);

                ResolveRef(replacement);

                _settings.TransformResolvedReplacement(replacement);

                if (refProperty.Parent?.Parent == null)
                    // When a property has already been replaced by recursions, it has no more Parent
                    continue;

                _settings.ApplyRefReplacement(_rootJObj, refProperty, replacement, refInfo.AbsoluteDocumentUri);
            }
        }

        private JToken GetRefJToken(RefInfo refInfo)
        {
            // TODO : Be able to load external (http) documents with credentials (swagger hub)
            var loader = GetReferenceLoader(refInfo);
            var replacement = loader.GetDocumentPart(refInfo.InDocumentPath);
            return replacement;
        }

        private JToken GetDocumentPart(string path)
        {
            EnsureRefResolved();

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

        internal RefInfo GetRefInfo(string refPath)
        {
            var parts = refPath.Split('#');
            if (parts.Length > 2)
                throw new ArgumentException(REF_KEYWORD + " value should not have more than 1 '#' char.", nameof(refPath));

            (bool embeded, string doc, string path) refParts;
            if (parts.Length == 1) // uri only
            {
                refParts = (false, parts[0], "");
            }
            else if (string.IsNullOrWhiteSpace(parts[0]))
            {
                refParts = (true, "", parts[1]);
            }
            else
            {
                refParts = (false, parts[0], parts[1]);
            }

            if (refParts.embeded)
                return new RefInfo(true, _documentUri.IsFile, _documentUri, refParts.path);

            var uri = new Uri(refParts.doc, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(_documentFolder, uri);

            return new RefInfo(false, uri.IsFile, uri, refParts.path);
        }
    }
}
