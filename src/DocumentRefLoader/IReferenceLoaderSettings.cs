using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace DocumentRefLoader
{
    public interface IReferenceLoaderSettings
    {
        string JsonSerialize(JObject jObject);
        string YamlSerialize(JObject jObject);

        bool ShouldResolveReference(RefInfo refInfo);


        void ApplyRefReplacement(JObject rootJObj, JProperty refProperty, JToken replacement);

        // Todo : add a tag/property to be able to discriminate "imported"/"merged" definitions ... (think x-exclude)
        void TransformResolvedReplacement(JToken jToken);

    }

    public class DefaultSettings : IReferenceLoaderSettings
    {
        public virtual bool ShouldResolveReference(RefInfo refInfo) => true;

        public virtual void ApplyRefReplacement(JObject rootJObj, JProperty refProperty, JToken replacement)
        {
            refProperty.Parent.Replace(replacement);
        }

        public virtual string JsonSerialize(JObject jObject) => jObject.ToString();

        public virtual string YamlSerialize(JObject jObject)
        {
            var deserializedObject = jObject.ToObject<ExpandoObject>(new JsonSerializer());
            return new Serializer().Serialize(deserializedObject);
        }

        public void TransformResolvedReplacement(JToken jToken)
        {
            // do nothing
        }

    }


    public class RawCopyNoRemoteSettings : DefaultSettings
    {
        public override bool ShouldResolveReference(RefInfo refInfo) => refInfo.IsLocal;
    }

    public class OpenApiV2MergeSettigns : DefaultSettings
    {
        public override bool ShouldResolveReference(RefInfo refInfo) => !refInfo.IsNestedInThisDocument;

        public virtual string SerializeToJson(JObject jObject) => GetSanitizedJsonString(base.JsonSerialize(jObject));

        private const string PATHS_KEYWORD = "paths";
        private const string DEFINITIONS_KEYWORD = "definitions";
        private const string PARAMETERS_KEYWORD = "parameters";
        private const string RESPONSES_KEYWORD = "responses";
        private const string X_EXCLUDE_KEYWORD = "x-exclude";
        private const string X_DEPENDENCIES_KEYWORD = "x-dependencies";

        readonly string[] _handledDefinitionTypes = new[] { DEFINITIONS_KEYWORD, PARAMETERS_KEYWORD, RESPONSES_KEYWORD };

        public override void ApplyRefReplacement(JObject rootJObj, JProperty refProperty, JToken replacement)
        {
            var refSplit = refProperty.Value.ToString().Split('/');

            var propertyPath = refProperty.Path;
            if (propertyPath.Contains(X_DEPENDENCIES_KEYWORD))
            {
                MergeAllElements(PATHS_KEYWORD, rootJObj, replacement);
                MergeAllElements(DEFINITIONS_KEYWORD, rootJObj, replacement);
                MergeAllElements(PARAMETERS_KEYWORD, rootJObj, replacement);
                MergeAllElements(PARAMETERS_KEYWORD, rootJObj, replacement);
            }
            else if (_handledDefinitionTypes.Any(t => propertyPath.Contains(t)))
            {
                var defType = refSplit[refSplit.Length - 2];
                var defName = refSplit.Last();
                replacement.Last?.AddAfterSelf(new JProperty(X_EXCLUDE_KEYWORD, "true"));
                TryAddElement(defType, rootJObj, defName, replacement);
                refProperty.Value = $"#/{defType}/{defName}";
            }
            else
            {
                throw new NotImplementedException($"Don't know how to handle replacement of a reference in path '{propertyPath}'");
            }
        }

        private static void MergeAllElements(string keyword, JObject rootJObj, JToken replacement)
        {
            if (replacement[keyword] is JContainer definitions)
            {
                foreach (var item in definitions.OfType<JProperty>())
                {
                    TryAddElement(keyword, rootJObj, item.Name, item.Value);
                }
            }
        }

        private static void TryAddElement(string collectionName, JObject rootJObj, string defName, JToken content)
        {
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
