//using HandlebarsDotNet;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Dynamic;
//using System.IO;
//using System.Linq;

//namespace CodegenUP.CustomHandlebars.Helpers
//{
//    /// <summary>
//    /// 
//    /// </summary>
//#if DEBUG
//    [HandlebarsHelperSpecification("{test: [{t: 'c'}, {t: 'a'}, {t: 'b', c:{ a:'a'} }]}", "{{#each test}}{{t}}{{/each}}", "cab")]
//    [HandlebarsHelperSpecification("[{t: 'c'}, {t: 'a'}, {t: 'b'}]", "{{#each_with_sort . 't'}}{{#each .}}{{t}}{{/each}}{{/each_with_sort}}", "abc")]
//    [HandlebarsHelperSpecification("[]", "{{#each_with_sort . .}}{{/each_with_sort}}", "")]
//    [HandlebarsHelperSpecification("{}", "{{#each_with_sort . .}}{{/each_with_sort}}", "")]
//    [HandlebarsHelperSpecification("{ c : {}, b : {} }", "{{#each_with_sort .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort}}", "bc")]
//    [HandlebarsHelperSpecification("{ b : {}, a : {} }", "{{#each_with_sort .}}{{#each .}}{{@key}}{{/each}}{{/each_with_sort}}", "ab")]
//    [HandlebarsHelperSpecification(GlobalSpecs.SWAGGER_SAMPLE, "{{#each_with_sort parameters}}{{#each .}}{{@key}},{{/each}}{{/each_with_sort}}", "accountIdParameter,credentialParameter,feedTypeParameter,marketplaceBusinessCodeParameter,publicationIdParameter,")]
//#endif
//    public class EachWithSort : SimpleBlockHelperBase
//    {
//        public EachWithSort() : base("each_with_sort") { }

//        public override HandlebarsBlockHelper Helper =>
//            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
//            {
//                EnsureArgumentsCountMin(arguments, 1);
//                EnsureArgumentsCountMax(arguments, 2);
//                Sort(output, options, context, arguments, SortDirection.Ascending, Name);
//            };

//        internal enum SortDirection
//        {
//            Ascending,
//            Descending
//        }

//        internal static void Sort(TextWriter output, HelperOptions options, object context, object[] arguments, SortDirection direction, string name)
//        {
//            var to_order = arguments[0];
//            var path = arguments.Length == 2 ? arguments[1]?.ToString() : null;
            
//            if (to_order is IEnumerable enumerable)
//            {
//                string keySelector(dynamic val) => (path == null) ? val?.ToString() : val?[path].ToString();

//                var orderable = enumerable.Cast<object>();

//                dynamic orderered = direction == SortDirection.Ascending
//                               ? orderable.OrderBy(keySelector)
//                               : orderable.OrderByDescending(keySelector)
//                               ;

//                var newContext = (orderered as IEnumerable).Cast<object>().ToArray();
//                options.Template(output, newContext);
//            }
//            else
//            {
//                options.Template(output, context);
//            }

//            //switch (to_order)
//            //{
//            //    //case JObject jObject:
//            //    //    {
//            //    //        JToken keySelector(KeyValuePair<string, JToken> t) => (path == null) ? t.Key : t.Value.SelectToken(path);
//            //    //        newContext = direction == SortDirection.Ascending
//            //    //            ? ((IDictionary<string, JToken>)jObject).OrderBy(keySelector)
//            //    //            : ((IDictionary<string, JToken>)jObject).OrderByDescending(keySelector)
//            //    //            ;
//            //    //    }
//            //    //    break;
//            //    //case JArray jArray:
//            //    //    {
//            //    //        if (path == null)
//            //    //            throw new CodeGenHelperException($"First argument being an Array, the second argument cannot be empty in the {name} helper.");

//            //    //        JToken keySelector(JToken t) => t.SelectToken(path);
//            //    //        newContext = direction == SortDirection.Ascending
//            //    //          ? jArray.OrderBy(keySelector)
//            //    //          : jArray.OrderByDescending(keySelector)
//            //    //          ;
//            //    //    }
//            //    //    break;
//            //    default:
//            //        throw new NotImplementedException($"{name} helper couldn't handle ordering on a {to_order?.GetType().Name} token.");
//            //}

//        }
//    }
//}
