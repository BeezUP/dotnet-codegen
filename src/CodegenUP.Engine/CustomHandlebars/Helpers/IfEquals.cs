using HandlebarsDotNet;
using System;
using System.IO;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// Execute template if the first argument is equal to any other argument, otherwise execute the inverse
    /// (all arguments are converted to string and case insensitive compared)
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#if_equals 'test' 'teSt'}}OK{{else}}{{/if_equals}}", "OK")]
    [HandlebarsHelperSpecification("{ a: '42', b: 42 }", "{{#if_equals a ./b }}OK{{else}}{{/if_equals}}", "OK")]
    [HandlebarsHelperSpecification("{}", "{{#if_equals 'test' 'NO'}}OK{{else}}NOK{{/if_equals}}", "NOK")]
    [HandlebarsHelperSpecification("{}", "{{#if_equals 'test' 'NO' 'NO' 'test'}}OK{{else}}NOK{{/if_equals}}", "OK")]
    [HandlebarsHelperSpecification("{}", "{{#if_equals 'test' 'NO' 'NOPE'}}OK{{else}}NOK{{/if_equals}}", "NOK")]
#endif
    public class IfEquals : SimpleBlockHelperBase
    {
        public IfEquals() : base("if_equals") { }

        public override void Helper(TextWriter output, HelperOptions options, object context, object[] arguments)
        {
            EnsureArgumentsCountMin(arguments, 2);

            var left = GetArgumentAs<string>(arguments, 0);

            for (int i = 1; i < arguments.Length; i++)
            {
                var right = GetArgumentAs<string>(arguments, i);
                if (string.Compare(left, right, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    options.Template(output, context);
                    return;
                }
            }

            options.Inverse(output, context);
        }
    }
}
