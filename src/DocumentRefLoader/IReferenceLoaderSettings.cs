using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentRefLoader
{
    public interface IReferenceLoaderSettings
    {
        JObject Deserialise(string jsonOrYaml, string sourceOriginalUri);

        string JsonSerialize(JObject jObject);
        string YamlSerialize(JObject jObject);

        bool ShouldResolveReference(RefInfo refInfo, ResolveRefState state);

        void ApplyRefReplacement(RefInfo refInfo, JObject rootJObj, JProperty refProperty, JToken replacement, Uri fromDocument);
    }
}
