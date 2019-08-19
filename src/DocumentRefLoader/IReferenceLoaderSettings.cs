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

        public override void ApplyRefReplacement(JObject rootJObj, JProperty refProperty, JToken replacement)
        {
            var refSplit = refProperty.Value.ToString().Split('/');

            var defType = refSplit[refSplit.Length - 2];
            var defName = refSplit.Last();

            if (defType == "definitions")
            {
                var currentDef = rootJObj["definitions"];
                if (!currentDef.Children<JProperty>().Any(x => x.Name == defName))
                {
                    if (currentDef.Last == null)
                    {
                        ((JObject)currentDef).Add(new JProperty(defName, replacement));
                    }
                    else
                    {
                        currentDef.Last.AddAfterSelf(new JProperty(defName, replacement));
                    }
                }
            }
            else if (defType == "parameters")
            {
                var currentParams = rootJObj["parameters"];
                if (currentParams == null)
                {
                    rootJObj.Add("parameters", new JObject { { defName, replacement } });
                }
                else if (!currentParams.Children<JProperty>().Any(x => x.Name == defName))
                {
                    currentParams.Last.AddAfterSelf(new JProperty(defName, replacement));
                }
            }
            else if (defType == "responses")
            {
                var currentResponses = rootJObj["responses"];
                if (currentResponses == null)
                {
                    rootJObj.Add("responses", new JObject { { defName, replacement } });
                }
                else if (!currentResponses.Children<JProperty>().Any(x => x.Name == defName))
                {
                    currentResponses.Last.AddAfterSelf(new JProperty(defName, replacement));
                }
            }
            else
            {
                throw new Exception($"'{defType}' definition type not handled.");
            }

            refProperty.Value = $"#/{defType}/{defName}";
        }
    }
}
