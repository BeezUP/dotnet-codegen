using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Dotnet.CodeGen.Schemas
{
    public interface ISchemaLoader
    {
        Task<JToken> LoadSchemaAsync(string documentUri);
    }
}
