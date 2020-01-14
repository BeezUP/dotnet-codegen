using Newtonsoft.Json.Linq;
using System.Linq;

namespace DocumentRefLoader
{
    public static class JObjectExtensions
    {
        public static OpenApiGeneralInfo GetOpenApiGeneralInfo(this JObject jObject)
        {
            return new OpenApiGeneralInfo
            {
                Title = jObject["info"]["title"]?.ToString(),
                BasePath = jObject["basePath"]?.ToString(),
                Tags = jObject["tags"]?.Children()["name"]?.Values<string>()?.ToArray()
            };
        }
    }
}
