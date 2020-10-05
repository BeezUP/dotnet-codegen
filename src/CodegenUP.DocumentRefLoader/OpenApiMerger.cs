using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DocumentRefLoader
{
    public class OpenApiMerger
    {
        readonly Action<JObject, OpenApiGeneralInfo[]> CustomizeDocument;
        readonly Action<(string basePath, string title, string itemType, JToken item)> CustomizeItem;
        ReferenceLoader[] Loaders { get; set; }
        bool IsResolved;
        JObject MergedRootJObject;

        public static OpenApiMerger GetMergerForDoc(string[] fileUris, OpenApiRefMergerOverridingValues overridingValues, string authorization = null)
        {
            void CustomizeDocument(JObject rootJObject, OpenApiGeneralInfo[] generalInfos)
            {
                if (rootJObject.ContainsKey("x-basePath")) rootJObject.Remove("x-basePath");
                if (rootJObject.ContainsKey("x-beezup-ops")) rootJObject.Remove("x-beezup-ops");
                if (rootJObject.ContainsKey("x-azure-api-id")) rootJObject.Remove("x-azure-api-id");

                var infoNode = (JObject)rootJObject["info"];

                if (overridingValues.Title != null) infoNode["title"] = overridingValues.Title;
                if (overridingValues.Description != null) infoNode["description"] = overridingValues.Description;

                if (overridingValues.LogoUrl != null)
                {
                    if (infoNode.ContainsKey("x-logo"))
                    {
                        infoNode["x-logo"]["url"] = overridingValues.LogoUrl;
                        if (overridingValues.LogoBackgroundColor != null) infoNode["x-logo"]["backgroundColor"] = overridingValues.LogoBackgroundColor;
                    }
                    else
                    {
                        infoNode.Add("x-logo", new JObject(new JProperty("url", overridingValues.LogoUrl), new JProperty("backgroundColor", overridingValues.LogoBackgroundColor ?? "")));
                    }
                }

                if (overridingValues.ContactEmail != null)
                {
                    if (infoNode.ContainsKey("contact"))
                    {
                        infoNode["contact"]["email"] = overridingValues.ContactEmail;
                    }
                    else
                    {
                        infoNode.Add("contact", new JObject(new JProperty("email", overridingValues.ContactEmail)));
                    }
                }

                if (overridingValues.LicenseName != null)
                {
                    if (infoNode.ContainsKey("license"))
                    {
                        infoNode["license"]["name"] = overridingValues.LicenseName;
                        if (overridingValues.LicenseUrl != null) infoNode["license"]["url"] = overridingValues.LicenseUrl;
                    }
                    else
                    {
                        infoNode.Add("license", new JObject(new JProperty("name", overridingValues.LicenseName), new JProperty("url", overridingValues.LicenseUrl ?? "")));
                    }
                }

                if (overridingValues.ExternalDocDescription != null) rootJObject.Add("externalDocs", new JObject(new JProperty("description", overridingValues.ExternalDocDescription), new JProperty("url", overridingValues.ExternalDocUrl ?? "")));


                if (overridingValues.Host != null) rootJObject["host"] = overridingValues.Host;

                if (overridingValues.BasePath != null) rootJObject["basePath"] = overridingValues.BasePath;

                var customTags = generalInfos
                    .Where(x => x.Tags != null)
                    .SelectMany(document => document.Tags.Select(tag => new
                    {
                        DisplayName = tag,
                        Name = $"{document.Title} - {tag}"
                    }))
                    .ToArray();

                rootJObject["tags"] = new JArray(customTags.Select(x => new JObject(new JProperty("name", x.Name), new JProperty("x-displayName", x.DisplayName))));

                var tagGroups = JArray.FromObject(generalInfos.Where(x => x.Tags != null).Select(document =>
                    new JObject(
                        new JProperty("name", document.Title),
                        new JProperty("tags", JArray.FromObject(document.Tags.Select(tag => $"{document.Title} - {tag}"))
                        ))));

                rootJObject.Add("x-tagGroups", tagGroups);

                var ok = new JProperty("api_key",
                    new JObject(
                        new JProperty("type", "apiKey"),
                        new JProperty("in", "header"),
                        new JProperty("name", "Ocp-Apim-Subscription-Key")
                    ));

                rootJObject.Add("securityDefinitions", new JObject(ok));
            }

            void CustomizeItem((string basePath, string title, string itemType, JToken item) tuple)
            {
                var (basePath, title, itemType, item) = tuple;

                if (itemType == "paths")
                {
                    foreach (var elem in item.Children().Children())
                    {
                        string[] tags = elem["tags"].Values<string>().Select(x => $"{title} - {x}").ToArray();

                        elem["tags"] = JArray.FromObject(tags);
                    }
                }
            }

            return new OpenApiMerger(
                        fileUris.Select(x => new Uri(x, UriKind.RelativeOrAbsolute)).ToArray(),
                        CustomizeDocument,
                        CustomizeItem,
                        authorization
                        );
        }

        public OpenApiMerger(
            Uri[] fileUris,
            Action<JObject, OpenApiGeneralInfo[]> customizeDocument = null,
            Action<(string basePath, string title, string itemType, JToken item)> customizeItem = null,
            string authorization = null
            )
        {
            CustomizeDocument = customizeDocument ?? delegate { };
            CustomizeItem = customizeItem ?? delegate { };
            var loadersCache = new Dictionary<Uri, ReferenceLoader>();
            Loaders = fileUris.Select(x => new ReferenceLoader(x, loadersCache, ReferenceLoaderStrategy.CopyRefContent, authorization)).ToArray();
        }


        public async Task<string> GetMergedJsonAsync() => (await GetMergedJObjectAsync()).ToString();

        public async Task<JObject> GetMergedJObjectAsync()
        {
            await EnsureAllDocsAreMergedAsync();
            return MergedRootJObject;
        }

        async Task EnsureAllDocsAreMergedAsync()
        {
            if (MergedRootJObject != null) return;
            MergedRootJObject = await MergeDocumentsAsync();
        }

        async Task<JObject> MergeDocumentsAsync()
        {
            if (Loaders.Length == 1)
                return await Loaders.First().GetRefResolvedJObjectAsync();

            var documents = new List<(ReferenceLoader refLoader, JObject jObj, OpenApiGeneralInfo openApiInfo)>();
            foreach (var loader in Loaders)
            {
                var jObj = await loader.GetRefResolvedJObjectAsync();
                var infos = jObj.GetOpenApiGeneralInfo();
                documents.Add((loader, jObj, infos));
            }

            var docs = documents.ToArray();
            var allPaths = GetAllItemsFromLoaders(docs, OpenApiConstants.PATHS_KEYWORD, true);
            var allDefs = GetAllItemsFromLoaders(docs, OpenApiConstants.DEFINITIONS_KEYWORD);
            var allParameters = GetAllItemsFromLoaders(docs, OpenApiConstants.PARAMETERS_KEYWORD);
            var allResponses = GetAllItemsFromLoaders(docs, OpenApiConstants.RESPONSES_KEYWORD);


            var newDocument = new JObject(documents.First().jObj);

            CopyItems(newDocument, OpenApiConstants.PATHS_KEYWORD, allPaths);
            CopyItems(newDocument, OpenApiConstants.DEFINITIONS_KEYWORD, allDefs);
            CopyItems(newDocument, OpenApiConstants.PARAMETERS_KEYWORD, allParameters);
            CopyItems(newDocument, OpenApiConstants.RESPONSES_KEYWORD, allResponses);

            CustomizeDocument(newDocument, documents.Select(x => x.openApiInfo).ToArray());

            return newDocument;
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

        void CopyItems(JObject rootJObjDest, string nodeName, Dictionary<string, JToken> itemsToCopy)
        {
            if (rootJObjDest.ContainsKey(nodeName)) rootJObjDest.Remove(nodeName);

            foreach (var kv in itemsToCopy)
            {
                CreateItem(rootJObjDest, nodeName, kv.Key, kv.Value);
            }
        }

        Dictionary<string, JToken> GetItemsFromLoader(JObject jObj, string itemType)
        {
            var items = jObj[itemType] as JContainer;
            return items?.OfType<JProperty>().ToDictionary(x => x.Name, y => y.Value) ?? new Dictionary<string, JToken>();
        }

        Dictionary<string, JToken> GetAllItemsFromLoaders((ReferenceLoader refLoader, JObject jObj, OpenApiGeneralInfo openApiInfo)[] loadersInfos, string itemType, bool trackDocumentOrigin = false)
        {
            return loadersInfos
                .SelectMany(infos =>
                {
                    var items = GetAllItemsFromLoader(infos, itemType, trackDocumentOrigin).ToArray();
                    foreach (var item in items)
                    {
                        CustomizeItem((infos.openApiInfo.BasePath, infos.openApiInfo.Title, itemType, item.Value));
                    }
                    return items;
                })
                .GroupBy(x => x.Key)
                .Select(x => x.First())
                .ToDictionary(x => x.Key, y => y.Value);
        }

        IEnumerable<KeyValuePair<string, JToken>> GetAllItemsFromLoader((ReferenceLoader refLoader, JObject jObj, OpenApiGeneralInfo openApiInfo) loaderInfos, string itemType, bool trackDocumentOrigin = false)
        {
            var currentItems = GetItemsFromLoader(loaderInfos.jObj, itemType)
                .Select(pair => new KeyValuePair<string, JToken>(
                    trackDocumentOrigin
                        ? $"{loaderInfos.openApiInfo.BasePath}{pair.Key}"
                        : pair.Key,
                    pair.Value));

            return currentItems;
        }
    }
}