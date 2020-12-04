using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// Remove empty lines from the inner template result
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#no_empty_lines}} {{/no_empty_lines}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#no_empty_lines}} |do not < remove please >| {{/no_empty_lines}}", " |do not < remove please >| \n")]
    [HandlebarsHelperSpecification("{}", "{{#no_empty_lines}} \n {{/no_empty_lines}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#no_empty_lines}} \r\n {{/no_empty_lines}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#no_empty_lines}} test{{/no_empty_lines}}", " test\n")]
    [HandlebarsHelperSpecification("{}", "{{#no_empty_lines}} a \n z {{/no_empty_lines}}", " a \n z \n")]
#endif
    public class NoEmptyLines : SimpleBlockHelperBase<object>
    {
        public NoEmptyLines() : base("no_empty_lines") { }

        public override void HelperFunction(TextWriter output, HelperOptions options, object? context, object[] otherArguments)
        {
            using var stream = new MemoryStream();
            using (var tw = new StreamWriter(stream, Encoding.Default, 500, true))
            {
                options.Template(tw, context);
            }
            stream.Seek(0, SeekOrigin.Begin);

            var sb = new StringBuilder();

            using var tr = new StreamReader(stream);
            for (var line = ""; line != null; line = tr.ReadLine())
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                sb.Append(line).Append('\n');
            }

            output.WriteSafeString(sb);
        }
    }
}
