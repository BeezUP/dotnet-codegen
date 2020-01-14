using HandlebarsDotNet;
using System.IO;

namespace Dotnet.CodeGen.CustomHandlebars.Helpers
{
    /// <summary>
    /// 
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("[{t: 'c'}, {t: 'a'}, {t: 'b'}]", "{{#each .}}{{t}}{{/each}}", "cab")]
    [HandlebarsHelperSpecification("[{t: 'c'}, {t: 'a'}, {t: 'b'}]", "{{#each_with_sort_inv . 't'}}{{#each .}}{{t}}{{/each}}{{/each_with_sort_inv}}", "cba")]
    [HandlebarsHelperSpecification("[]", "{{#each_with_sort_inv . .}}{{/each_with_sort_inv}}", "")]
    [HandlebarsHelperSpecification("{ a : {}, b : {} }", "{{#each_with_sort_inv .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort_inv}}", "ba")]
    [HandlebarsHelperSpecification("{ b : {}, a : {} }", "{{#each_with_sort_inv .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort_inv}}", "ba")]
    [HandlebarsHelperSpecification(GlobalSpecs.SWAGGER_SAMPLE, "{{#each_with_sort_inv parameters}}{{#each .}}{{@key}},{{/each}}{{/each_with_sort_inv}}", "publicationIdParameter,marketplaceBusinessCodeParameter,feedTypeParameter,credentialParameter,accountIdParameter,")]
#endif
    public class EachWithSortInv : SimpleBlockHelperBase
    {
        public EachWithSortInv() : base("each_with_sort_inv") { }

        public override HandlebarsBlockHelper Helper =>
            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
            {
                EnsureArgumentsCountMin(arguments, 1);
                EnsureArgumentsCountMax(arguments, 2);
                EachWithSort.Sort(output, options, context, arguments, EachWithSort.SortDirection.Descending, Name);
            };
    }
}
