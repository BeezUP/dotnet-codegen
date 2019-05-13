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
    [HandlebarsHelperSpecification("{}", "{{#if_empty ''}}OK{{else}}{{/if_empty}}", "OK")]
    [HandlebarsHelperSpecification("{}", "{{#if_empty 'test'}}OK{{else}}NOK{{/if_empty}}", "NOK")]
#endif
    public class IfEmpty : SimpleBlockHelperBase
    {
        public IfEmpty() : base("if_empty") { }

        public override HandlebarsBlockHelper Helper =>
            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
            {
                EnsureArgumentsCount(arguments, 1);

                var arg = GetArgumentStringValue(arguments, 0) ?? "";

                if (string.IsNullOrWhiteSpace(arg))
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
