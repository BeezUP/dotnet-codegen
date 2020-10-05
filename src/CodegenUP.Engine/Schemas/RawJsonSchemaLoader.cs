using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodegenUP.Schemas
{
    public class RawJsonSchemaLoader : ISchemaLoader
    {
        public async Task<JToken> LoadSchemaAsync(IEnumerable<string> documentUris, string? authorization)
        {
            var docs = documentUris.ToArray();
            if (docs.Length != 1)
                throw new ArgumentException($"Cannot load multiple OpenApiSchema using this loader [{nameof(RawJsonSchemaLoader)}]", nameof(documentUris));
            var documentUri = docs[0];

            var document = File.ReadAllText(documentUri);
            return JToken.Parse(document);
        }
    }
}
