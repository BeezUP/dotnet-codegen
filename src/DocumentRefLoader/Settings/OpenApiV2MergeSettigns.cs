using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization.EventEmitters;

namespace DocumentRefLoader.Settings
{
    public class OpenApiV2MergeSettigns : DefaultSettings
    {
        public override bool ShouldResolveReference(RefInfo refInfo) => !refInfo.IsNestedInThisDocument;

        public virtual string SerializeToJson(JObject jObject) => GetSanitizedJsonString(base.JsonSerialize(jObject));

        private const string TAGS_KEYWORD = "tags";
        private const string TAGS_NAME_KEYWORD = "name";
        private const string TAGS_DESC_KEYWORD = "description";
        private const string PATHS_KEYWORD = "paths";
        private const string BASEPATH_KEYWORD = "basePath";
        private const string DEFINITIONS_KEYWORD = "definitions";
        private const string PARAMETERS_KEYWORD = "parameters";
        private const string RESPONSES_KEYWORD = "responses";
        private const string X_EXCLUDE_KEYWORD = "x-exclude";
        private const string X_DEPENDENCIES_KEYWORD = "x-dependencies";

        readonly string[] _handledDefinitionTypes = new[] { DEFINITIONS_KEYWORD, PARAMETERS_KEYWORD, RESPONSES_KEYWORD };

        public override void ApplyRefReplacement(JObject rootJObj, JProperty refProperty, JToken replacement, Uri fromDocument)
        {
            var refSplit = refProperty.Value.ToString().Split('/');

            var propertyPath = refProperty.Path;
            if (propertyPath.Contains(X_DEPENDENCIES_KEYWORD))
            {
                var basePath = replacement[BASEPATH_KEYWORD]?.ToString();
                var basePathTag = basePath?.TrimStart('/'); // remove '/' at the begining of the base path

                var basePathTagMerge = basePathTag != null;
                if (basePathTagMerge)
                {
                    if (!(rootJObj[TAGS_KEYWORD] is JArray globalTags))
                        rootJObj[TAGS_KEYWORD] = globalTags = new JArray();

                    if (!(globalTags.FirstOrDefault(t => t[TAGS_NAME_KEYWORD] == (JToken)basePathTag) is JObject))
                        globalTags.Add(new JObject { { TAGS_NAME_KEYWORD, basePathTag }, { TAGS_DESC_KEYWORD, $"{basePath} - {fromDocument}" } });
                }

                var transformProperty = basePathTagMerge
                   ? (Action<JProperty>)(jProp =>
                   {
                       foreach (var tags in jProp.Descendants().OfType<JProperty>().Where(p => p.Name == TAGS_KEYWORD))
                           tags.Value = new JArray { basePathTag };
                   })
                   : null;
                MergeAllProperties(PATHS_KEYWORD, rootJObj, replacement, false, transformProperty);

                MergeAllProperties(DEFINITIONS_KEYWORD, rootJObj, replacement, true);
                MergeAllProperties(PARAMETERS_KEYWORD, rootJObj, replacement, true);
                MergeAllProperties(PARAMETERS_KEYWORD, rootJObj, replacement, true);
            }
            else if (_handledDefinitionTypes.Any(t => propertyPath.Contains(t)))
            {
                var defType = refSplit[refSplit.Length - 2];
                var defName = refSplit.Last();
                TryAddElement(defType, rootJObj, defName, replacement, true);
                refProperty.Value = $"#/{defType}/{defName}";
            }
            else
            {
                throw new NotImplementedException($"Don't know how to handle replacement of a reference in path '{propertyPath}'");
            }
        }

        private static void MergeAllProperties(string keyword, JObject rootJObj, JToken replacement, bool addExclude, Action<JProperty> transformProperty = null)
        {
            if (replacement[keyword] is JContainer definitions)
            {
                foreach (var jprop in definitions.OfType<JProperty>())
                {
                    transformProperty?.Invoke(jprop);
                    TryAddElement(keyword, rootJObj, jprop.Name, jprop.Value, addExclude);
                }
            }
        }

        private static void TryAddElement(string collectionName, JObject rootJObj, string defName, JToken content, bool addExclude)
        {
            if (addExclude && !content.Children<JProperty>().Any(p => p.Name == X_EXCLUDE_KEYWORD))
            {
                content.Last?.AddAfterSelf(new JProperty(X_EXCLUDE_KEYWORD, true));
            }

            if (!(rootJObj[collectionName] is JObject collection))
            {
                rootJObj.Add(collectionName, new JObject { { defName, content } });
            }
            else if (!collection.Children<JProperty>().Any(x => x.Name == defName))
            {
                if (collection.Last == null)
                {
                    collection.Add(new JProperty(defName, content));
                }
                else
                {
                    collection.Last.AddAfterSelf(new JProperty(defName, content));
                }
            }
        }

        private string GetSanitizedJsonString(string json)
        {
            json = json
                .Replace("\"x-exclude\": \"true\"", "\"x-exclude\": true")
                .Replace("\"required\": \"true\"", "\"required\": true")
                .Replace("\"required\": \"false\"", "\"required\": false");

            {
                var matchs = Regex.Matches(json, "(?:\"minimum\": \")(.*?)(?:\",)");
                foreach (Match match in matchs)
                {
                    var tmp = match.Value.Replace(": \"", ": ").Replace("\",", ","); // "minimum": "true", => "minimum": true,
                    json = json.Replace(match.Value, tmp);
                }
            }

            {
                var matchs = Regex.Matches(json, "(?:\"minimum\": \")(.*?)(?:\")");
                foreach (Match match in matchs)
                {
                    var tmp = match.Value.Replace(": \"", ": ");    // "minimum": "123"     => "minimum": 123"
                    tmp = tmp.Substring(0, tmp.Length - 1);         // "minimum": 123"      => "minimum": 123
                    json = json.Replace(match.Value, tmp);
                }
            }

            return json;
        }
    }
}
