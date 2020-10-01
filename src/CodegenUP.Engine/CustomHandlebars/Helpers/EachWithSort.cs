using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// 
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("[{t: 'c'}, {t: 'a'}, {t: 'b'}]", "{{#each .}}{{t}}{{/each}}", "cab")]
    [HandlebarsHelperSpecification("[{t: 'c'}, {t: 'a'}, {t: 'b'}]", "{{#each_with_sort . 't'}}{{#each .}}{{t}}{{/each}}{{/each_with_sort}}", "abc")]
    [HandlebarsHelperSpecification("[]", "{{#each_with_sort . .}}{{/each_with_sort}}", "")]
    [HandlebarsHelperSpecification("{ a : {}, b : {} }", "{{#each_with_sort .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort}}", "ab")]
    [HandlebarsHelperSpecification("{ b : {}, a : {} }", "{{#each_with_sort .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort}}", "ab")]
    [HandlebarsHelperSpecification(GlobalSpecs.SWAGGER_SAMPLE, "{{#each_with_sort parameters}}{{#each .}}{{@key}},{{/each}}{{/each_with_sort}}", "accountIdParameter,credentialParameter,feedTypeParameter,marketplaceBusinessCodeParameter,publicationIdParameter,")]
#endif
    public class EachWithSort : SimpleBlockHelperBase
    {
        public EachWithSort() : base("each_with_sort") { }

        public override HandlebarsBlockHelper Helper =>
            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
            {
                EnsureArgumentsCountMin(arguments, 1);
                EnsureArgumentsCountMax(arguments, 2);
                Sort(output, options, context, arguments, SortDirection.Ascending, Name);
            };

        internal enum SortDirection
        {
            Ascending,
            Descending
        }

        internal static void Sort(TextWriter output, HelperOptions options, object context, object[] arguments, SortDirection direction, string name)
        {
            var to_order = arguments[0];
            if (!(to_order is IEnumerable enumerable))
                options.Template(output, context);

            var jPath = arguments.Length == 2 ? arguments[1]?.ToString() : null;

            object newContext;
            switch (to_order)
            {
                case JObject jObject:
                    {
                        JToken keySelector(KeyValuePair<string, JToken> t) => (jPath == null) ? t.Key : t.Value.SelectToken(jPath) ?? JValue.CreateNull();
                        newContext = direction == SortDirection.Ascending
                            ? ((IDictionary<string, JToken>)jObject).OrderBy(keySelector)
                            : ((IDictionary<string, JToken>)jObject).OrderByDescending(keySelector)
                            ;
                    }
                    break;
                case JArray jArray:
                    {
                        if (jPath == null)
                            throw new CodeGenHelperException($"First argument being an Array, the second argument cannot be empty in the {name} helper.");

                        JToken keySelector(JToken t) => t.SelectToken(jPath) ?? JValue.CreateNull();
                        newContext = direction == SortDirection.Ascending
                          ? jArray.OrderBy(keySelector)
                          : jArray.OrderByDescending(keySelector)
                          ;
                    }
                    break;
                default:
                    throw new NotImplementedException($"{name} helper couldn't handle ordering on a {to_order?.GetType().Name} token.");
            }

            options.Template(output, newContext);
        }
    }
}
