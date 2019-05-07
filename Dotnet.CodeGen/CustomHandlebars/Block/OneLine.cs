using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Dotnet.CodeGen.CustomHandlebars.Block
{

    /// <summary>
    /// Append a single line, no matter how much the inner template is producing line breaks
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#one_line}} \n{{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}} \n {{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}\n {{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}\n{{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}} \r\n {{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}\r\n{{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}test\r\n\r\n\r\ntest{{/one_line}}", "test      test")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}{{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}   test {{/one_line}}", "test")]
    [HandlebarsHelperSpecification("{}", "{{#one_line 5}}test{{/one_line}}", "     test")]
#endif
    public class OneLine : BlockHelperBase
    {
        static readonly Regex regex = new Regex(@" *[\r\n?|\n] *", RegexOptions.Compiled);

        public OneLine() : base("one_line") { }

        public override HandlebarsBlockHelper Helper =>
            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
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
            };
    }
}
