using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace DocumentRefLoader
{
    public interface IReferenceLoaderSettings
    {
        bool DisableRemoteReferenceLoading { get; set; }

        string JsonSerialize(JObject jObject);
        string YamlSerialize(JObject jObject);
        void ApplyRefReplacement(JObject rootJObj, JProperty refProperty, JToken replacement);
    }

    public class DefaultSettings : IReferenceLoaderSettings
    {
        public virtual bool DisableRemoteReferenceLoading { get; set; }

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
    }


    public class RawCopyNoRemoteSettings : DefaultSettings
    {
        public RawCopyNoRemoteSettings()
        {
            DisableRemoteReferenceLoading = true;
        }
    }

    public class OpenApiV2MergeSettigns : DefaultSettings
    {
        public virtual string SerializeToJson(JObject jObject) => base.JsonSerialize(jObject).Replace("\"x-exclude\": \"true\"", "\"x-exclude\": true");

        readonly string[] _handledDefinitionTypes = new[] { "definitions", "parameters", "responses" };

        public override void ApplyRefReplacement(JObject rootJObj, JProperty refProperty, JToken replacement)
        {
            var refSplit = refProperty.Value.ToString().Split('/');

            var defType = refSplit[refSplit.Length - 2];
            var defName = refSplit.Last();

            if (_handledDefinitionTypes.Contains(defType))
            {
                AddElement(defType, rootJObj, defName, replacement);
            }
            else
            {
                throw new InvalidOperationException($"'{defType}' definition type not handled.");
            }

            refProperty.Value = $"#/{defType}/{defName}";
        }

        private static void AddElement(string collectionName, JObject rootJObj, string defName, JToken content)
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
    }
}
