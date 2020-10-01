using HandlebarsDotNet;
using System.IO;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// Execute template only if argument is not empty
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#if_not_empty ''}}{{else}}OK{{/if_not_empty}}", "OK")]
    [HandlebarsHelperSpecification("{}", "{{#if_not_empty 'test'}}NOK{{else}}OK{{/if_not_empty}}", "NOK")]
#endif
    public class IfNotEmpty : SimpleBlockHelperBase
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
