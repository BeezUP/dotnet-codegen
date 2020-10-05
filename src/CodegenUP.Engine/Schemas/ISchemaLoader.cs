using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodegenUP.Schemas
{
    public interface ISchemaLoader
    {
        Task<JToken> LoadSchemaAsync(IEnumerable<string> documentUris, string? authorization);
    }
}
