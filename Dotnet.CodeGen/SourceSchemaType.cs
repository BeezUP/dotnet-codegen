using System;
using System.Collections.Generic;
using System.Text;
using Dotnet.CodeGen.Schemas;

namespace Dotnet.CodeGen.CodeGen
{
    public enum SourceSchemaType
    {
        RawJson,
        Swagger,
        OpenApi,
        GraphQl,
        RawXml
    }

    public static class SchemaTypeExtensions
    {
        public const SourceSchemaType DEFAULT_SHEMA_TYPE = SourceSchemaType.OpenApi;

        public static ISchemaLoader GetSchemaLoader(this SourceSchemaType schemaType)
        {
            switch (schemaType)
            {
                case SourceSchemaType.RawJson:
                    return new RawJsonSchemaLoader();
                case SourceSchemaType.Swagger:
                    return new SwaggerSchemaLoader();
                case SourceSchemaType.OpenApi:
                    return new OpenApiSchemaLoader();
                default:
                    throw new NotImplementedException($"Todo : implement {schemaType}SchemaLoader and use it in {nameof(SchemaTypeExtensions)}.{nameof(GetSchemaLoader)}.");
            }
        }
    }
}
