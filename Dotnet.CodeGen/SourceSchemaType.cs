using System;
using System.Collections.Generic;
using System.Text;
using Dotnet.CodeGen.CodeGen.Schemas;

namespace Dotnet.CodeGen.CodeGen
{
    public enum SourceSchemaType
    {
        RawJson,
        OpenApi,
        GraphQl,
        RawXml
    }

    public static class SchemaTypeExtensions
    {
        public static ISchemaLoader GetSchemaLoader(this SourceSchemaType schemaType)
        {
            switch (schemaType)
            {
                case SourceSchemaType.RawJson:
                    return new RawJsonSchemaLoader();
                case SourceSchemaType.OpenApi:
                    return new OpenApiSchemaLoader();
                default:
                    throw new NotImplementedException($"Todo : implement {schemaType}SchemaLoader and use it in {nameof(SchemaTypeExtensions)}.{nameof(GetSchemaLoader)}.");
            }
        }
    }
}
