using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentRefLoader
{
    public interface IReferenceLoaderSettings
    {
        string JsonSerialize(JObject jObject);
        string YamlSerialize(JObject jObject);

        bool ShouldResolveReference(RefInfo refInfo);


        void ApplyRefReplacement(RefInfo refInfo, JObject rootJObj, JProperty refProperty, JToken replacement, Uri fromDocument);

        // Todo : add a tag/property to be able to discriminate "imported"/"merged" definitions ... (think x-exclude)
        void TransformResolvedReplacement(JToken jToken);

    }
}
