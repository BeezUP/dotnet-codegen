using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;

namespace Dotnet.CodeGen.Schemas
{
    public class SwaggerSchemaLoader : ISchemaLoader
    {
        public JToken LoadSchema(string documentUri)
        {
            var loader = new DocumentRefLoader(documentUri, DocumentRefLoaderStrategy.OpenApiV2Merge);
            var jObj = loader.GetRefResolvedJObject();
            return jObj;
        }
    }


    public enum DocumentRefLoaderStrategy
    {
        RawCopy,
        OpenApiV2Merge
    }

    /// <summary>
    /// https://swagger.io/docs/specification/using-ref/
    /// </summary>
    public class DocumentRefLoader
    {
        public const string REF_KEYWORD = "$ref";
        private readonly Dictionary<Uri, DocumentRefLoader> _otherLoaders;
        private readonly DocumentRefLoaderStrategy _strategy;
        private readonly Uri _documentUri;
        private readonly string _originalDocument;
        private readonly Uri _documentFolder;
        private readonly JObject _rootJObj;

        //static HttpClient s_httpClient = new HttpClient();

        //private readonly bool _rootIsJson;
        //private readonly bool _rootIsYaml;

        private static readonly Deserializer s_yamlDeserializer = new Deserializer();
        private static readonly Serializer s_yamlSerializer = new Serializer();

        public DocumentRefLoader(string fileUri, DocumentRefLoaderStrategy strategy = DocumentRefLoaderStrategy.RawCopy)
            : this(new Uri(fileUri, UriKind.RelativeOrAbsolute), null, strategy)
        { }

        private DocumentRefLoader(Uri documentUri, Dictionary<Uri, DocumentRefLoader> otherLoaders, DocumentRefLoaderStrategy strategy)
        {
            _documentUri = documentUri;
            if (!_documentUri.IsAbsoluteUri)
                _documentUri = new Uri(new Uri(Path.Combine(Directory.GetCurrentDirectory(), ".")), _documentUri);
            _documentFolder = new Uri(_documentUri, ".");

            _otherLoaders = otherLoaders ?? new Dictionary<Uri, DocumentRefLoader>() { { _documentUri, this } };
            _strategy = strategy;
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

        //private JsonConverter _expConverter = new ExpandoObjectConverter();
        private JsonSerializer _jsonSerializer = new JsonSerializer();
        public string GetRefResolvedYaml()
        {
            var jObj = GetRefResolvedJObject();

            //JsonConvert.DeserializeObject<ExpandoObject>(json, _expConverter);
            dynamic deserializedObject = jObj.ToObject<ExpandoObject>(_jsonSerializer);

            return s_yamlSerializer.Serialize(deserializedObject);
        }

        public string GetRefResolvedJson()
        {
            switch (_strategy)
            {
                case DocumentRefLoaderStrategy.RawCopy:
                    return GetRefResolvedJObject().ToString();
                case DocumentRefLoaderStrategy.OpenApiV2Merge:
                    return GetRefResolvedJObject().ToString().Replace("\"x-exclude\": \"true\"", "\"x-exclude\": true");
                default:
                    throw new ArgumentOutOfRangeException(nameof(_strategy));
            }
        }

        public Dictionary<string, JToken> Parameters = new Dictionary<string, JToken>();
        public Dictionary<string, JToken> Responses = new Dictionary<string, JToken>();
        public Dictionary<string, JToken> Definitions = new Dictionary<string, JToken>();


        public JObject GetRefResolvedJObject()
        {
            EnsureRefResolved();

            if (_strategy == DocumentRefLoaderStrategy.OpenApiV2Merge)
            {
                var allDef = _otherLoaders
                  .SelectMany(x => x.Value.Definitions)
                  .Concat(Definitions)
                  .GroupBy(x => x.Key)
                  .Select(x => x.First())
                  .ToDictionary(x => x.Key, y => y.Value);

                var currentDef = _rootJObj["definitions"];

                foreach (var def in allDef)
                {
                    try
                    {
                        if (!currentDef.Children<JProperty>().Any(x => x.Name == def.Key))
                        {
                            if (currentDef.Last == null)
                            {
                                ((JObject)currentDef).Add(new JProperty(def.Key, def.Value));
                            }
                            else
                            {
                                currentDef.Last.AddAfterSelf(new JProperty(def.Key, def.Value));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }

                var allParams = _otherLoaders
                    .SelectMany(x => x.Value.Parameters)
                    .Concat(Parameters)
                    .GroupBy(x => x.Key)
                    .Select(x => x.First())
                    .ToDictionary(x => x.Key, y => y.Value);

                var currentParams = _rootJObj["parameters"];

                foreach (var param in allParams)
                {
                    if (currentParams == null)
                    {
                        _rootJObj.Add("parameters", new JObject { { param.Key, param.Value } });
                        currentParams = _rootJObj["parameters"];
                    }
                    else if (!currentParams.Children<JProperty>().Any(x => x.Name == param.Key))
                    {
                        currentParams.Last.AddAfterSelf(new JProperty(param.Key, param.Value));
                    }
                }

                var allResps = _otherLoaders
                    .SelectMany(x => x.Value.Responses)
                    .Concat(Responses)
                    .GroupBy(x => x.Key)
                    .Select(x => x.First())
                    .ToDictionary(x => x.Key, y => y.Value);

                var currentResponses = _rootJObj["responses"];

                foreach (var resp in allResps)
                {
                    if (currentResponses == null)
                    {
                        _rootJObj.Add("responses", new JObject { { resp.Key, resp.Value } });
                        currentResponses = _rootJObj["responses"];
                    }
                    else if (!currentResponses.Children<JProperty>().Any(x => x.Name == resp.Key))
                    {
                        currentResponses.Last.AddAfterSelf(new JProperty(resp.Key, resp.Value));
                    }
                }
            }

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

        private JToken ResolveRef(JToken token)
        {
            if (!(token is JContainer container))
                return token;

            var refProps = container.Descendants()
                .OfType<JProperty>()
                .Where(p => p.Name == REF_KEYWORD)
                .ToList();

            foreach (var refProp in refProps)
            {
                var replacement = GetRefJToken(refProp.Value.ToString());
                if (replacement == null)
                    continue;

                ResolveRef(replacement);

                if (refProp.Parent?.Parent == null)
                    // When a property has already been replaced by recursions, it has no more Parent
                    continue;

                switch (_strategy)
                {
                    case DocumentRefLoaderStrategy.RawCopy:
                        refProp.Parent.Replace(replacement);
                        break;
                    case DocumentRefLoaderStrategy.OpenApiV2Merge:
                        var refSplit = refProp.Value.ToString().Split('/');

                        var defType = refSplit[refSplit.Length - 2];
                        var defName = refSplit.Last();


                        if (defType == "definitions")
                        {
                            if (!Definitions.ContainsKey(defName))
                            {
                                Definitions.Add(defName, replacement);
                            }
                        }
                        else if (defType == "parameters")
                        {
                            if (!Parameters.ContainsKey(defName))
                            {
                                Parameters.Add(defName, replacement);
                            }
                        }
                        else if (defType == "responses")
                        {
                            if (!Responses.ContainsKey(defName))
                            {
                                Responses.Add(defName, replacement);
                            }
                        }
                        else
                        {
                            throw new Exception("bfiohfhifweiofhweiofhweiohf");
                        }

                        refProp.Value = $"#/{defType}/{defName}";
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(_strategy));
                }
            }

            return container;
        }

        private JToken GetRefJToken(string refPath)
        {
            var refInfo = GetRefInfo(refPath);
            //if (!refInfo.IsLocal)
            //    return null; // TODO : Be able to load external (http) documents with credentials (swagger hub)

            var loader = GetYamlLoader(refInfo);
            var replacement = loader.GetDocumentPart(refInfo.InDocumentPath, loader != this);

            return replacement;
        }

        private JToken GetDocumentPart(string path, bool ensureResolved)
        {
            if (ensureResolved)
                EnsureRefResolved();

            var parts = path.Split('/');

            JToken token = _rootJObj;
            foreach (var part in parts)
            {
                if (!string.IsNullOrWhiteSpace(part))
                {
                    token = token[part];
                }
            }

            return token;
        }


        private DocumentRefLoader GetYamlLoader(RefInfo refInfo)
        {
            if (refInfo.IsNestedInThisDocument)
                return this;

            if (_otherLoaders.TryGetValue(refInfo.AbsoluteDocumentUri, out var loader))
                return loader;

            loader = new DocumentRefLoader(refInfo.AbsoluteDocumentUri, _otherLoaders, _strategy);
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

        internal class RefInfo
        {
            public RefInfo(bool isNested, bool isLocal, Uri absoluteUri, string path)
            {
                IsNestedInThisDocument = isNested;
                IsLocal = isLocal;
                AbsoluteDocumentUri = absoluteUri;
                InDocumentPath = path;
            }

            public bool IsNestedInThisDocument { get; }
            public bool IsLocal { get; }
            public Uri AbsoluteDocumentUri { get; }
            public string InDocumentPath { get; }
        }
    }
}
