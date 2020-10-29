﻿using HandlebarsDotNet;
using System;
using System.IO;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// Performs a string comparison between 2 arguments
    /// (all arguments are converted to string and case insensitive compared)
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#if_equals 'test' 'teSt'}}OK{{else}}{{/if_equals}}", "OK")]
    [HandlebarsHelperSpecification("{ a: '42', b: 42 }", "{{#if_equals a ./b }}OK{{else}}{{/if_equals}}", "OK")]
    [HandlebarsHelperSpecification("{}", "{{#if_equals 'test' 'NO'}}OK{{else}}NOK{{/if_equals}}", "NOK")]
#endif
    public class IfEquals : SimpleBlockHelperBase<object, string, string>
    {
        public IfEquals() : base("if_equals") { }

        public override void HelperFunction(TextWriter output, HelperOptions options, object context, string arg1, string arg2, object[] otherArguments)
        {
            if (string.Compare(arg1, arg2, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                options.Template(output, context);
            }
            else
            {
                options.Inverse(output, context);
            }
        }
    }
}