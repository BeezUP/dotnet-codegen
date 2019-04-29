using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Dotnet.CodeGen.CodeGen.Schemas
{
    public interface ISchemaLoader
    {
        JToken LoadSchema(string documentUri);
    }
}
