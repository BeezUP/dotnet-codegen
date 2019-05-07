using System;
using System.Collections.Generic;
using System.Text;
using Dotnet.CodeGen.Schemas;

namespace Dotnet.CodeGen.CodeGen
{
    public enum SourceSchemaType
    {
        /// <summary>
        /// Json document, no transformation
        /// </summary>
        RawJson,
        /// <summary>
        /// Swagger v2 documents. Merging remote references
        /// </summary>
        Swagger,
        /// <summary>
        /// Microsoft.OpenApi librairy capabilities. Input OpenApi v2 or v3 and output full resolved references OpenApi v3 json specification
        /// /!\ NOT WORKING YET, DO NOT USE ! : https://github.com/microsoft/OpenAPI.NET/issues/406
        /// </summary>
        OpenApi,
        /// <summary>
        /// GraphQL spec
        /// (Not Implemented)
        /// </summary>
        GraphQl,
        /// <summary>
        /// XML to Json
        /// (Not Implemented)
        /// </summary>
        RawXml
    }

    public static class SchemaTypeExtensions
    {
        public const SourceSchemaType DEFAULT_SHEMA_TYPE = SourceSchemaType.Swagger;

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
