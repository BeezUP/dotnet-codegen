using HandlebarsDotNet;
using System;
using System.IO;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// Execute template if the first argument is not equal to all other arguments, otherwise execute the inverse
    /// (all arguments are converted to string and case insensitive compared)
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#if_not_equals 'test' 'teSt'}}{{else}}NOK{{/if_not_equals}}", "NOK")]
    [HandlebarsHelperSpecification("{ a: '42', b: 42 }", "{{#if_not_equals a ./b }}{{else}}NOK{{/if_not_equals}}", "NOK")]
    [HandlebarsHelperSpecification("{}", "{{#if_not_equals 'test' 'NO'}}OK{{else}}NOK{{/if_not_equals}}", "OK")]
    [HandlebarsHelperSpecification("{}", "{{#if_not_equals 'test' 'NO' 'NO' 'test'}}OK{{else}}NOK{{/if_not_equals}}", "NOK")]
    [HandlebarsHelperSpecification("{}", "{{#if_not_equals 'test' 'NO' 'NOPE'}}OK{{else}}NOK{{/if_not_equals}}", "OK")]
#endif
    public class IfNotEquals : SimpleBlockHelperBase
    {
        public IfNotEquals() : base("if_not_equals") { }

        public override void Helper(TextWriter output, HelperOptions options, object context, object[] arguments)
        {
            EnsureArgumentsCountMin(arguments, 2);

            var left = GetArgumentAs<string>(arguments, 0);

            for (int i = 1; i < arguments.Length; i++)
            {
                var right = GetArgumentAs<string>(arguments, i);
                if (string.Compare(left, right, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    options.Inverse(output, context);
                    return;
                }
            }

            options.Template(output, context);
        }
    }
}
