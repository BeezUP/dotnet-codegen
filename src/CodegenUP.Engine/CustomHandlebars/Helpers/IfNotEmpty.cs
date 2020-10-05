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
    public class IfNotEmpty : SimpleBlockHelperBase<object, string>
    {
        public IfNotEmpty() : base("if_not_empty") { }

        public override void HelperFunction(TextWriter output, HelperOptions options, object context, string arg, object[] otherArguments)
        {
            if (string.IsNullOrWhiteSpace(arg))
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
