using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars.Block
{
    //[HandlebarsHelperSpecification("{ test: 'AA' }", "{{uppercase_first_letter test}}", "AA")]
    public class IfAreEqual : BlockHelperBase
    {
        public IfAreEqual() : base("if_are_equal") { }

        public override HandlebarsBlockHelper Helper =>
            (TextWriter output, HelperOptions options, dynamic context, object[] arguments) =>
            {
                EnsureArgumentsCount(arguments, 2);

                string arg;
                if (arguments[0] is JValue t)
                {
                    arg = t.Value.ToString();
                }
                else
                {
                    arg = arguments[0] as string;
                }

                var expected = arguments[1] as string;

                if (arg == expected)
                {
                    options.Template(output, context);
                }
                else
                {
                    options.Inverse(output, context);
                }
            };
    }
}
