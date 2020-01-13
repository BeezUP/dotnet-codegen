using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Dotnet.CodeGen.Schemas
{
    public class RawJsonSchemaLoader : ISchemaLoader
    {
        public async Task<JToken> LoadSchemaAsync(string documentUri)
        {
            var document = File.ReadAllText(documentUri);
            return JToken.Parse(document);
        }
    }
}
