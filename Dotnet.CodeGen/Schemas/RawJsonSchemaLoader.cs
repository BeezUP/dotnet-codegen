using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Dotnet.CodeGen.CodeGen.Schemas
{
    public class RawJsonSchemaLoader : ISchemaLoader
    {
        public JObject LoadSchema(string documentUri)
        {
            var document = File.ReadAllText(documentUri);
            return JObject.Parse(document);
        }
    }
}
