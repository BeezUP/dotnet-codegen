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
    public class AreEqual : BlockHelperBase
    {
        public AreEqual() :
            base(
                "are_equal",
                () => (TextWriter output, HelperOptions options, dynamic context, object[] arguments) =>
                {
                    if (arguments.Length != 2)
                    {
                        options.Inverse(output, context);
                    }
                    else
                    {
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
                    }
                })
        {
        }
    }
}
