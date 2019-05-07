using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Dotnet.CodeGen.CustomHandlebars.Block
{

    /// <summary>
    /// Append on one line, no matter how mush the inner template is producing line breaks
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#one_line}}\n{{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}\r\n{{/one_line}}", "")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}test\n\r\n\r\ntest{{/one_line}}", "testtest")]
    [HandlebarsHelperSpecification("{}", "{{#one_line}}{{/one_line}}", "")]
#endif
    public class OneLine : BlockHelperBase
    {
        static readonly Regex regex = new Regex(@" *[\r\n?|\n\r?] *", RegexOptions.Compiled);

        public OneLine() : base("one_line") { }

        public override HandlebarsBlockHelper Helper =>
            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
            {
                EnsureArgumentsCount(arguments, 0);
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
                        //result = result.Replace("\r", "").Replace("\n", "");
                        result = regex.Replace(result, " ");
                        output.WriteSafeString(result);
                    }
                }
            };
    }
}
