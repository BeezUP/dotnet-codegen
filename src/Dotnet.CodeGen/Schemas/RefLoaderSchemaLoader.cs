using DocumentRefLoader;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnet.CodeGen.Schemas
{
    public class RefLoaderSchemaLoader : ISchemaLoader
    {
        public async Task<JToken> LoadSchemaAsync(IEnumerable<string> documentUris, string? authorization)
        {
            var docs = documentUris.ToArray();

            if (docs.Length == 1)
            {
                var loader = new ReferenceLoader(docs[0], ReferenceLoaderStrategy.CopyRefContent, authorization);
                var jObj = await loader.GetRefResolvedJObjectAsync();
                return jObj;
            }
            else
            {
                var merger = new OpenApiMerger(documentUris.Select(u => u.GetAbsoluteUri()).ToArray(), authorization: authorization);
                var jObj = await merger.GetMergedJObjectAsync();
                return jObj;
            }
        }
    }
}
