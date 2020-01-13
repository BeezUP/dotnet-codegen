using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;

namespace DocumentRefLoader
{
    public class OpenApiRefResolver
    {
        internal readonly Uri DocumentUri;
        readonly Uri DocumentFolder;
        readonly string Authorization;

        bool IsDownloaded;
        bool IsResolved;
        bool IsRead;

        string OriginalDocument;
        internal JObject RootJObj;

        internal OpenApiGeneralInfo GeneralInfo;

        internal readonly Dictionary<Uri, OpenApiRefResolver> OtherResolvers;

        static readonly JsonSerializer JsonSerializer = new JsonSerializer();
        static readonly Deserializer YamlDeserializer = new Deserializer();
        static readonly Serializer YamlSerializer = new Serializer();

        public OpenApiRefResolver(Uri documentUri, string authorization = null, Dictionary<Uri, OpenApiRefResolver> otherResolvers = null)
        {
            DocumentUri = documentUri.IsAbsoluteUri
                ? documentUri
                : new Uri(new Uri(Path.Combine(Directory.GetCurrentDirectory(), ".")), DocumentUri);

            DocumentFolder = new Uri(DocumentUri, ".");

            Authorization = authorization;

            OtherResolvers = otherResolvers ?? new Dictionary<Uri, OpenApiRefResolver>();
        }

        async Task EnsureIsDownloadedAsync()
        {
            if (IsDownloaded) return;
            await DownloadFileAsync();
            IsDownloaded = true;
        }

        async Task DownloadFileAsync()
        {
            Console.WriteLine($"Downloading: {DocumentUri}");

            using (var webClient = new WebClient())
            {
                if (Authorization != null)
                {
                    webClient.Headers.Add("Authorization", Authorization);
                }

                OriginalDocument = await webClient.DownloadStringTaskAsync(DocumentUri);
            }
        }

        async Task EnsureIsReadAsync()
        {
            if (IsRead) return;
            await ReadDocumentAsync();
            IsRead = true;
        }

        async Task ReadDocumentAsync()
        {
            await EnsureIsDownloadedAsync();

            object yamlObject;
            using (var sr = new StringReader(OriginalDocument))
            {
                yamlObject = YamlDeserializer.Deserialize(sr);
            }
            var json = JsonConvert.SerializeObject(yamlObject);

            RootJObj = JObject.Parse(json);

            GeneralInfo = new OpenApiGeneralInfo
            {
                Title = RootJObj["info"]["title"]?.ToString(),
                BasePath = RootJObj["basePath"]?.ToString(),
                Tags = RootJObj["tags"]?.Children()["name"]?.Values<string>()?.ToArray()
            };
        }

        internal async Task EnsureIsResolvedAsync()
        {
            if (IsResolved) return;
            await ResolveDocumentAsync();
            IsResolved = true;
        }

        OpenApiRefResolver GetYamlLoader(OpenApiRefInfo refInfo)
        {
            if (refInfo.IsNestedInThisDocument)
                return this;

            if (refInfo.AbsoluteDocumentUri == DocumentUri)
                return this;

            if (OtherResolvers.TryGetValue(refInfo.AbsoluteDocumentUri, out var loader))
                return loader;

            loader = new OpenApiRefResolver(refInfo.AbsoluteDocumentUri, Authorization, OtherResolvers);
            OtherResolvers[refInfo.AbsoluteDocumentUri] = loader;
            return loader;
        }

        async Task<JToken> GetDocumentPartAsync(string path, bool ensureRead)
        {
            if (ensureRead)
                await EnsureIsReadAsync();

            var parts = path.Split('/');

            JToken token = RootJObj;

            foreach (var part in parts)
            {
                if (!string.IsNullOrWhiteSpace(part))
                {
                    token = token[part];
                }
            }

            return token;
        }

        public class RefPropertyHolder
        {
            public string PropertyType { get; set; }

            public string PropertyName { get; set; }

            public JToken PropertyValue { get; set; }

            public RefPropertyHolder[] SubProperties { get; set; }
        }

        void CreateItem(JObject rootJObjDest, string nodeName, string itemName, JToken itemValue)
        {
            var node = rootJObjDest[nodeName];

            if (node == null)
            {
                rootJObjDest.Add(nodeName, new JObject { { itemName, itemValue } });
            }
            else if (node.Children<JProperty>().All(x => x.Name != itemName))
            {
                var jProperty = new JProperty(itemName, itemValue);

                if (node.Last == null)
                {
                    ((JObject)node).Add(jProperty);
                }
                else
                {
                    node.Last.AddAfterSelf(jProperty);
                }
            }
        }

        Dictionary<string, RefPropertyHolder> GetSubProperties(OpenApiRefResolver loader, JContainer container)
        {
            var refProps = container
                .Descendants()
                .OfType<JProperty>()
                .Where(p => p.Name == Constants.REF_KEYWORD)
                .ToList();

            return refProps.Select(async refProp =>
            {
                var subValue = await loader.GetRefJTokenAsync(refProp.Value.ToString());
                refProp.Value = $"#/{subValue.PropertyType}/{subValue.PropertyName}";
                return subValue;
            })
            .Select(x => x.Result).Where(x => x != null)
            .GroupBy(x => x.PropertyName)
            .Select(x => x.FirstOrDefault(y => y.PropertyValue != null))
            .Where(x => x != null)
            .ToDictionary(x => x.PropertyName, y => y);
        }

        public async Task<RefPropertyHolder> GetRefJTokenAsync(string refPath)
        {
            var refInfo = OpenApiRefInfo.GetRefInfo(DocumentUri, DocumentFolder, refPath);
            var loader = GetYamlLoader(refInfo);
            var propertyValue = await loader.GetDocumentPartAsync(refInfo.InDocumentPath, true);

            var refSplit = refPath.Split('/');

            var propertyType = refSplit[refSplit.Length - 2]; //example: definitions
            var propertyName = refSplit.Last();

            Dictionary<string, RefPropertyHolder> subProperties = propertyValue is JContainer container
                ? GetSubProperties(loader, container)
                : null;

            return new RefPropertyHolder
            {
                PropertyName = propertyName,
                PropertyType = propertyType,
                PropertyValue = propertyValue,
                SubProperties = subProperties?.Values.ToArray()
            };
        }

        RefPropertyHolder[] FlattenSubProperties(RefPropertyHolder[] subProperties)
        {
            if (subProperties == null || subProperties.Length == 0) return subProperties;
            return subProperties.Concat(FlattenSubProperties(subProperties.Where(x => x.SubProperties != null).SelectMany(x => x.SubProperties).ToArray())).ToArray();
        }

        async Task ResolveDocumentAsync()
        {
            await EnsureIsReadAsync();

            if (!(RootJObj is JContainer container)) return;

            Dictionary<string, RefPropertyHolder> subProperties = GetSubProperties(this, container);
            var flattened = FlattenSubProperties(subProperties.Values.ToArray());

            foreach (var propertyHolder in flattened)
            {
                CreateItem(RootJObj, propertyHolder.PropertyType, propertyHolder.PropertyName, propertyHolder.PropertyValue);
            }
        }

        public async Task<JObject> GetRefResolvedJObjectAsync()
        {
            await EnsureIsResolvedAsync();
            return RootJObj;
        }

        public async Task<string> GetRefResolvedJsonAsync()
        {
            return (await GetRefResolvedJObjectAsync()).ToString();
        }

        public async Task<string> GetRefResolvedYamlAsync()
        {
            var jObj = await GetRefResolvedJObjectAsync();

            dynamic deserializedObject = jObj.ToObject<ExpandoObject>(JsonSerializer);

            return YamlSerializer.Serialize(deserializedObject);
        }
    }

}