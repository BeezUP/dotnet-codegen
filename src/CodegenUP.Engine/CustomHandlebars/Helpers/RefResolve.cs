using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// If a '$ref' property is found, it will be resolved and will replace the context object.
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification(GlobalSpecs.SWAGGER_SAMPLE, "{{#each paths}}{{#each this}}{{#each parameters}}{{#ref_resolve}}{{ name }},{{/ref_resolve}}{{/each}}{{/each}}{{/each}}", "marketplaceBusinessCode,marketplaceBusinessCode,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,request,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,request,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,request,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,marketplaceBusinessCode,accountId,publicationId,x-BeezUP-Credential,")]
#endif
    public class RefResolve : SimpleBlockHelperBase
    {
        public const string REF = "$ref";

        public RefResolve() : base("ref_resolve") { }

        public override void Helper(TextWriter output, HelperOptions options, object context, object[] arguments)
        {
            EnsureArgumentsCount(arguments, 0);

            var jObj = context as JObject ?? throw new CodeGenHelperException($"{Name} helper needs a {nameof(JObject)} as context.");

            var refProp = jObj.SelectToken(REF, false) as JValue;
            if (refProp == null)
            {
                options.Template(output, context);
                return;
            }

            var refPath = refProp?.ToString();
            if (string.IsNullOrWhiteSpace(refPath))
                throw new CodeGenHelperException($"{Name} helper: {REF} property was found but without value.");

            if (refPath == null || string.IsNullOrWhiteSpace(refPath))
            {
                throw new CodeGenHelperException($"{REF} value was empty or null.");
            }

            var jsonPath = RefPathToJsonPath(refPath);
            var resolved = jObj.Root.SelectToken(jsonPath) as JObject
                ?? throw new CodeGenHelperException($"{Name} helper: unable to resolve {refPath} as an object ({nameof(JObject)}).");

            options.Template(output, resolved);
        }

        static string RefPathToJsonPath(string @ref) => @ref.Replace("/", ".").Trim('#');
    }
}
