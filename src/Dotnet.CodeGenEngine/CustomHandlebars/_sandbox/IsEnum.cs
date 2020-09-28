//using HandlebarsDotNet;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;

//namespace Dotnet.CodeGen.CustomHandlebars.Block
//{
//    //[HandlebarsHelperSpecification("{ test: 'AA' }", "{{uppercase_first_letter test}}", "AA")]
//    public class IsEnum : SimpleBlockHelperBase
//    {
//        public IsEnum() : base("is_enum") { }

//        public override HandlebarsBlockHelper Helper =>
//            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
//            {
//                var enumProperty = ((JObject)context)?.Property("enum");

//                if (enumProperty != null)
//                {
//                    options.Template(output, context);
//                }
//                else
//                {
//                    options.Inverse(output, context);
//                }
//            };
//    }
//}
