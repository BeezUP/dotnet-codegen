using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using YamlDotNet.Serialization;

namespace DocumentRefLoader.Settings
{
    public class DefaultSettings : IReferenceLoaderSettings
    {
        public virtual bool ShouldResolveReference(RefInfo refInfo) => true;

        public virtual void ApplyRefReplacement(JObject rootJObj, JProperty refProperty, JToken replacement, Uri fromDocument)
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
}
