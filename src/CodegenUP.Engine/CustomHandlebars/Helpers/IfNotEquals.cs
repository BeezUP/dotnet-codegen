using HandlebarsDotNet;
using System;
using System.IO;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// Performs a string comparison between 2 arguments
    /// (all arguments are converted to string and case insensitive compared)
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#if_not_equals 'test' 'teSt'}}{{else}}NOK{{/if_not_equals}}", "NOK")]
    [HandlebarsHelperSpecification("{ a: '42', b: 42 }", "{{#if_not_equals a ./b }}{{else}}NOK{{/if_not_equals}}", "NOK")]
    [HandlebarsHelperSpecification("{}", "{{#if_not_equals 'test' 'NO'}}OK{{else}}NOK{{/if_not_equals}}", "OK")]
#endif
    public class IfNotEquals : SimpleBlockHelperBase
    {
        public IfNotEquals() : base("if_not_equals") { }

        public override void Helper(TextWriter output, HelperOptions options, object context, object[] arguments)
        {
            EnsureArgumentsCount(arguments, 2);

            var arg1 = GetArgumentAs<string>(arguments, 0) ?? "";
            var arg2 = GetArgumentAs<string>(arguments, 1) ?? "";

            if (string.Compare(arg1, arg2, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                options.Inverse(output, context);
            }
            else
            {
                options.Template(output, context);
            }
        }
    }
}
