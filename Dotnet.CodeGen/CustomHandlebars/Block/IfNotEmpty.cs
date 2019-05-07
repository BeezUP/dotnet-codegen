using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars.Block
{
    /// <summary>
    /// Performs a string comparison between 2 arguments
    /// (all arguments are converted to string and case insensitive compared)
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#if_not_empty ''}}{{else}}OK{{/if_not_empty}}", "OK")]
    [HandlebarsHelperSpecification("{}", "{{#if_not_empty 'test'}}NOK{{else}}OK{{/if_not_empty}}", "NOK")]
#endif
    public class IfNotEmpty : BlockHelperBase
    {
        public IfNotEmpty() : base("if_not_empty") { }

        public override HandlebarsBlockHelper Helper =>
            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
            {
                EnsureArgumentsCount(arguments, 1);

                var arg = GetArgumentStringValue(arguments, 0) ?? "";

                if (string.IsNullOrWhiteSpace(arg))
                {
                    options.Inverse(output, context);
                }
                else
                {
                    options.Template(output, context);
                }
            };
    }
}
