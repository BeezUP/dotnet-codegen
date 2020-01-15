using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnet.CodeGen.Schemas
{
    public interface ISchemaLoader
    {
        Task<JToken> LoadSchemaAsync(IEnumerable<string> documentUris);
    }
}
