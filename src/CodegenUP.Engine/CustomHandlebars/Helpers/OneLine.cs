using HandlebarsDotNet;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CodegenUP.CustomHandlebars.Helpers
{

    /// <summary>
    /// Append a single line, taking out empty lines & meaningless whitespaces from the inner template
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#one_line}} {{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}} \n {{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}\n {{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}\n{{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}} \r\n {{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}\r\n{{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}} test{{/one_line}}", "test")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}} a \n z {{/one_line}}", "a z")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}a\n z{{/one_line}}", "a z")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}a\nz{{/one_line}}", "a z")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}a \r\n z{{/one_line}}", "a z")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}a \r\n \r\n \r\nz{{/one_line}}", "a z")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}test\r\n\r\n\r\ntest{{/one_line}}", "test test")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}{{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}   test {{/one_line}}", "test")]
    [HandlebarsHelperSpecification("{}", "{{#one_line 5}}test{{/one_line}}", "     test")]
#endif
    public class OneLine : SimpleBlockHelperBase
    {
        static readonly Regex regex = new Regex(@"(?: *[\r\n?|\n] *)+", RegexOptions.Compiled);

        public OneLine() : base("one_line") { }

        public override void Helper(TextWriter output, HelperOptions options, object context, object[] arguments)
        {
            EnsureArgumentsCountMax(arguments, 1);

            var indent = 0;
            if (arguments.Length != 0)
            {
                if (int.TryParse(arguments[0]?.ToString(), out var i))
                    indent = i;
            }

            using (var stream = new MemoryStream())
            {
                using (var tw = new StreamWriter(stream, Encoding.Default, 500, true))
                {
                    options.Template(tw, context);
                }
                stream.Seek(0, SeekOrigin.Begin);
                using (var tr = new StreamReader(stream))
                {
                    var result = tr.ReadToEnd();
                    result = regex.Replace(result, " ");
                    result = new string(' ', indent) + result.Trim();
                    output.WriteSafeString(result);
                }
            }
        }
    }
}
