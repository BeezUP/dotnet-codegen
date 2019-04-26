//using HandlebarsDotNet;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;

//namespace Dotnet.CodeGen.CustomHandlebars.Block
//{
//    public static partial class SPECS
//    {
//        public const string EachWithSort_TEST_DOCUMENT = @"
//        {
//            'type' : 'object',
//            'required' : [ 'errorMeSSage', 'test' ],
//            'properties' : {
//                'errorMessage' : {
//                    'type' : 'string'
//                },
//                'non_required_prop' : {
//                    'type' : 'int'
//                }
//            }
//        }";
//    }

//    [HandlebarsHelperSpecification(SPECS.EachWithSort_TEST_DOCUMENT, "{{#each_with_sort parameters}} {{.}} {{/each_with_sort}}", "AA")]
//    public class EachWithSort : BlockHelperBase
//    {
//        public EachWithSort() : base("each_with_sort") { }

//        public override HandlebarsBlockHelper Helper =>
//            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
//            {
//                var children = ((JObject)arguments.First()).Children().Select(x => (JProperty)x).ToArray();

//                var ret = new JObject(children.OrderBy(x => x.Name));

//                options.Template(output, ret);
//            };
//    }
//}
