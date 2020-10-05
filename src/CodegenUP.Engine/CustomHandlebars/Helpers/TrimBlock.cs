using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HandlebarsDotNet;

namespace CodegenUP.CustomHandlebars.Helpers
{
    public abstract class TrimBlockBase : SimpleBlockHelperBase
    {
        private readonly Func<string, char[], string> _trimFunc;

        public TrimBlockBase(string name, Func<string, char[], string> trimFunc) : base(name)
        {
            _trimFunc = trimFunc;
        }

        public override void Helper(TextWriter output, HelperOptions options, object context, object[] arguments)
        {
            var trimChars = TryGetArgumentAsString(arguments, 0, out var str)
                   ? str.ToArray()
                   : new[] { ' ' };

            Trim(output, options, context, (str) => _trimFunc(str, trimChars));
        }

        void Trim(TextWriter output, HelperOptions options, object context, Func<string, string> trimFunc)
        {
            using var stream = new MemoryStream();
            using (var tw = new StreamWriter(stream, Encoding.Default, 500, true))
            {
                options.Template(tw, context);
            }
            stream.Seek(0, SeekOrigin.Begin);
            using var tr = new StreamReader(stream);
            var result = tr.ReadToEnd();
            output.WriteSafeString(trimFunc(result));
        }
    }


    /// <summary>
    /// Trim start and end of a block output
    /// (all arguments are converted to string and case insensitive compared)
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#trim_block ' '}} 1,2,3,4 {{/trim_block}}", "1,2,3,4")]
    [HandlebarsHelperSpecification("{}", "{{#trim_block ','}}1,2,3,4{{/trim_block}}", "1,2,3,4")]
    [HandlebarsHelperSpecification("{}", "{{#trim_block ','}}1,2,3,4,{{/trim_block}}", "1,2,3,4")]
    [HandlebarsHelperSpecification("{}", "{{#trim_block ','}},1,2,3,4,{{/trim_block}}", "1,2,3,4")]
    [HandlebarsHelperSpecification("{}", "{{#trim_block ','}},,1,2,3,4,,{{/trim_block}}", "1,2,3,4")]
    [HandlebarsHelperSpecification("{ a: '42', b: 42, c: 42 }", "{{#trim_block ','}}{{#each this}}{{@key}},{{/each}}{{/trim_block}}", "a,b,c")]
#endif
    public class TrimBlock : TrimBlockBase
    {
        public TrimBlock() : base("trim_block", (str, trimChar) => str.Trim(trimChar)) { }
    }

    /// <summary>
    /// Trim start of a block output
    /// (all arguments are converted to string and case insensitive compared)
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#trim_block_start}} 1,2,3,4 {{/trim_block_start}}", "1,2,3,4 ")]
    [HandlebarsHelperSpecification("{}", "{{#trim_block_start ','}}1,2,3,4{{/trim_block_start}}", "1,2,3,4")]
    [HandlebarsHelperSpecification("{}", "{{#trim_block_start ','}}1,2,3,4,{{/trim_block_start}}", "1,2,3,4,")]
    [HandlebarsHelperSpecification("{}", "{{#trim_block_start ','}},1,2,3,4,{{/trim_block_start}}", "1,2,3,4,")]
    [HandlebarsHelperSpecification("{}", "{{#trim_block_start ','}},,1,2,3,4,,{{/trim_block_start}}", "1,2,3,4,,")]
    [HandlebarsHelperSpecification("{ a: '42', b: 42, c: 42 }", "{{#trim_block_start ','}}{{#each this}}{{@key}},{{/each}}{{/trim_block_start}}", "a,b,c,")]
#endif
    public class TrimBlockStart : TrimBlockBase
    {
        public TrimBlockStart() : base("trim_block_start", (str, trimChar) => str.TrimStart(trimChar)) { }
    }

    /// <summary>
    /// Trim end of a block output
    /// (all arguments are converted to string and case insensitive compared)
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#trim_block_end}} 1,2,3,4 {{/trim_block_end}}", " 1,2,3,4")]
    [HandlebarsHelperSpecification("{}", "{{#trim_block_end ','}}1,2,3,4{{/trim_block_end}}", "1,2,3,4")]
    [HandlebarsHelperSpecification("{}", "{{#trim_block_end ','}}1,2,3,4,{{/trim_block_end}}", "1,2,3,4")]
    [HandlebarsHelperSpecification("{}", "{{#trim_block_end ','}},1,2,3,4,{{/trim_block_end}}", ",1,2,3,4")]
    [HandlebarsHelperSpecification("{}", "{{#trim_block_end ','}},,1,2,3,4,,{{/trim_block_end}}", ",,1,2,3,4")]
    [HandlebarsHelperSpecification("{ a: '42', b: 42, c: 42 }", "{{#trim_block_end ','}}{{#each this}}{{@key}},{{/each}}{{/trim_block_end}}", "a,b,c")]
#endif
    public class TrimBlockEnd : TrimBlockBase
    {
        public TrimBlockEnd() : base("trim_block_end", (str, trimChar) => str.TrimEnd(trimChar)) { }
    }
}
