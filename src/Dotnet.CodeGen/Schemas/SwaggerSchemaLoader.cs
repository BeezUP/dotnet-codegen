using DocumentRefLoader;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Dotnet.CodeGen.Schemas
{
    public class SwaggerSchemaLoader : ISchemaLoader
    {
        public async Task<JToken> LoadSchemaAsync(string documentUri)
        {
            var loader = new ReferenceLoader(documentUri, ReferenceLoaderStrategy.OpenApiV2Merge);
            var jObj = await loader.GetRefResolvedJObjectAsync();
            return jObj;
        }
    }
}
