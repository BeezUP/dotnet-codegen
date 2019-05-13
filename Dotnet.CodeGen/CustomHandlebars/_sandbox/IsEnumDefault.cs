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
//    public class IsEnumDefault : SimpleBlockHelperBase
//    {
//        public IsEnumDefault() : base("is_enum_default") { }

//        public override HandlebarsBlockHelper Helper =>
//            (TextWriter output, HelperOptions options, dynamic context, object[] arguments) =>
//            {
//                var argument = arguments.First() as string;

//                var definition = (JObject)context.Parent.Parent.Parent;
//                var enumProperty = definition.Property("enum");
//                var defaultProperty = definition.Property("default");

//                if (enumProperty != null
//                    && defaultProperty?.Value.ToString() == argument)
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
