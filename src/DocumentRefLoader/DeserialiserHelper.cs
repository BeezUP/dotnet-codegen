using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace DocumentRefLoader
{
    public static class DeserialiserHelper
    {
        public static JObject Deserialise(string jsonOrYaml, string fileName)
        {
            return (GetFileNameFromUrl(fileName).EndsWith("json"))
                ? DeserialiseJson(jsonOrYaml)
                : DeserialiseYaml(jsonOrYaml)
                ;
        }

        private static readonly Deserializer s_yamlDeserializer = new Deserializer();

        public static JObject DeserialiseYaml(string yaml)
        {
            object yamlObject;
            using (var sr = new StringReader(yaml))
            {
                yamlObject = s_yamlDeserializer.Deserialize(sr);
            }
            var json = JsonConvert.SerializeObject(yamlObject);
            return DeserialiseJson(json);
        }

        public static JObject DeserialiseJson(string json)
        {
            return JObject.Parse(json);
        }


        
        readonly static Uri SomeBaseUri = new Uri("http://canbeanything");

        /// <summary>
        /// Stackoverflow coding : https://stackoverflow.com/questions/1105593/get-file-name-from-uri-string-in-c-sharp
        /// </summary>
        static string GetFileNameFromUrl(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                uri = new Uri(SomeBaseUri, url);

            return Path.GetFileName(uri.LocalPath);
        }
    }
}
