﻿using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars.Helpers
{
    /// <summary>
    /// Determines whether the beginning of the second argumentmatches the second one (case sensitive)
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#start_with 'test' 'test-one'}}OK{{else}}{{/start_with}}", "OK")]
    [HandlebarsHelperSpecification("{}", "{{#start_with 'test' 'one-test'}}OK{{else}}NOK{{/start_with}}", "NOK")]
    [HandlebarsHelperSpecification("{one: 'test-one', two: 'one-test'}", "{{#start_with 'test' one}}OK{{else}}{{/start_with}}", "OK")]
    [HandlebarsHelperSpecification("{one: 'test-one', two: 'one-test'}", "{{#start_with 'test' two}}OK{{else}}NOK{{/start_with}}", "NOK")]
#endif
    public class StartWith : SimpleBlockHelperBase
    {
        public StartWith() : base("start_with") { }

        public override HandlebarsBlockHelper Helper =>
            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
            {
                EnsureArgumentsCount(arguments, 2);

                var arg1 = GetArgumentStringValue(arguments, 0) ?? "";
                var arg2 = GetArgumentStringValue(arguments, 1) ?? "";

                if (arg2.StartsWith(arg1))
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
