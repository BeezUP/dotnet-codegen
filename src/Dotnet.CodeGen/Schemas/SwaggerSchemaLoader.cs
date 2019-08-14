using System.Text;
using DocumentRefLoader;
using Newtonsoft.Json.Linq;

namespace Dotnet.CodeGen.Schemas
{
    public class SwaggerSchemaLoader : ISchemaLoader
    {
        public JToken LoadSchema(string documentUri)
        {
            var loader = new ReferenceLoader(documentUri, ReferenceLoaderStrategy.OpenApiV2Merge);
            var jObj = loader.GetRefResolvedJObject();
            return jObj;
        }
    }
}
