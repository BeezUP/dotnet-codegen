using DocumentRefLoader.Yaml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;
using YamlDotNet.Serialization.NodeDeserializers;
using YamlDotNet.Serialization.NodeTypeResolvers;

namespace DocumentRefLoader.Settings
{
    public class DefaultSettings : IReferenceLoaderSettings
    {
        public JObject Deserialise(string jsonOrYaml, string sourceOriginalUri)
        {
            return (Helper.GetFileNameFromUrl(sourceOriginalUri).EndsWith("json"))
                ? DeserialiseJson(jsonOrYaml)
                : DeserialiseYaml(jsonOrYaml)
                ;
        }

        private static readonly IDeserializer _yamlDeserializer = new DeserializerBuilder()
            //.WithNodeDeserializer<ForcedTypesNodeDeserializer>(inner => new ForcedTypesNodeDeserializer(inner), s => s.InsteadOf<ObjectNodeDeserializer>())
            .WithNodeTypeResolver<ForcedNodeTypeResolver>(inner => new ForcedNodeTypeResolver(inner), s => s.InsteadOf<YamlConvertibleTypeResolver>())
            .Build()
            ;

        protected static JObject DeserialiseYaml(string yaml)
        {
            object yamlObject;
            using (var sr = new StringReader(yaml))
            {
                yamlObject = _yamlDeserializer.Deserialize(sr);
            }
            var json = JsonConvert.SerializeObject(yamlObject);
            return DeserialiseJson(json);
        }

        protected static JObject DeserialiseJson(string json)
        {
            return JObject.Parse(json);
        }

        public virtual bool ShouldResolveReference(RefInfo refInfo) => true;

        public virtual void ApplyRefReplacement(RefInfo refInfo, JObject rootJObj, JProperty refProperty, JToken replacement, Uri fromDocument)
        {
            refProperty.Parent.Replace(replacement);
        }

        public virtual string JsonSerialize(JObject jObject) => jObject.ToString();

        readonly ISerializer _yamlSerializer = new SerializerBuilder()
            .WithEventEmitter(next => new ForceQuotedStringValuesEventEmitter(next))
            .Build();

        public virtual string YamlSerialize(JObject jObject)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new ExpandoObjectConverter());

            var deserializedObject = jObject.ToObject<ExpandoObject>();
            var yaml = _yamlSerializer.Serialize(deserializedObject);
            return yaml;
        }
    }
}
