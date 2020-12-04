using HandlebarsDotNet;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CodegenUP.CustomHandlebars.Helpers
{

    /// <summary>
    /// Append a single line, taking out empty lines & meaningless whitespaces from the inner template
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#one_line}} {{/one_line}}", "\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}} |do not < remove please >| {{/one_line}}", "|do not < remove please >|\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}} \n {{/one_line}}", "\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}\n {{/one_line}}", "\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}\n{{/one_line}}", "\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}} \r\n {{/one_line}}", "\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}\r\n{{/one_line}}", "\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}} test{{/one_line}}", "test\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}} a \n z {{/one_line}}", "az\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}a\n z{{/one_line}}", "az\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}a\nz{{/one_line}}", "az\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}a \r\n z{{/one_line}}", "az\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}a \r\n \r\n \r\nz{{/one_line}}", "az\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}test\r\n\r\n\r\ntest{{/one_line}}", "testtest\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line 0 'true'}}test\r\n\r\n\r\ntest{{/one_line}}", "testtest\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line 0 'false'}}test\r\n\r\n\r\ntest{{/one_line}}", "testtest")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}{{/one_line}}", "\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}   test {{/one_line}}", "test\n")]
    [HandlebarsHelperSpecification("{}", "{{#one_line 5}}test{{/one_line}}", "     test\n")]
#endif
    public class OneLine : SimpleBlockHelperBase<object, int?, bool?>
    {

        public OneLine() : base("one_line") { }

        public override void HelperFunction(TextWriter output, HelperOptions options, object context, int? indent, bool? lineBreak, object[] otherArguments)
        {
            EnsureArgumentsCountMax(otherArguments, 0);

            using var stream = new MemoryStream();
            using (var tw = new StreamWriter(stream, Encoding.Default, 500, true))
            {
                options.Template(tw, context);
            }
            stream.Seek(0, SeekOrigin.Begin);

            using var tr = new StreamReader(stream);
            var result = tr.ReadToEnd();
            result = StringHelpers.OnOneLine(result, indent, lineBreak);
            output.WriteSafeString(result);
        }


    }
}
