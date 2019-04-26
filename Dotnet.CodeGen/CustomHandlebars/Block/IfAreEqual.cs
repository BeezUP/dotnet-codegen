using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars.Block
{
    /// <summary>
    /// Performs a string comparison between 2 arguments
    /// (all arguments are converted to string and case insensitive compared)
    /// </summary>
    [HandlebarsHelperSpecification("{}", "{{#if_are_equal 'test' 'teSt'}}OK{{else}}{{/if_are_equal}}", "OK")]
    [HandlebarsHelperSpecification("{ a: '42', b: 42 }", "{{#if_are_equal a ./b }}OK{{else}}{{/if_are_equal}}", "OK")]
    [HandlebarsHelperSpecification("{}", "{{#if_are_equal 'test' 'NO'}}OK{{else}}NOK{{/if_are_equal}}", "NOK")]
    public class IfAreEqual : BlockHelperBase
    {
        public IfAreEqual() : base("if_are_equal") { }

        public override HandlebarsBlockHelper Helper =>
            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
            {
                EnsureArgumentsCount(arguments, 2);

                var arg1 = GetArgumentStringValue(arguments, 0);
                var arg2 = GetArgumentStringValue(arguments, 1);

                if (string.Compare(arg1, arg2, StringComparison.InvariantCultureIgnoreCase) == 0)
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
