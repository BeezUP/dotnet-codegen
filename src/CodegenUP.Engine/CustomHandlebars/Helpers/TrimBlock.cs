using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HandlebarsDotNet;

namespace CodegenUP.CustomHandlebars.Helpers
{
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
    public class TrimBlock : SimpleBlockHelperBase
    {
        public TrimBlock() : base("trim_block") { }

        public override HandlebarsBlockHelper Helper =>
           (TextWriter output, HelperOptions options, object context, object[] arguments) =>
           {
               var trimChar = GetArgumentCharValueOrDefault(arguments, 0, ' ');

               TrimBlockHelper.Trim(output, options, context, (str) => str.Trim(trimChar));

           };

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
    public class TrimBlockStart : SimpleBlockHelperBase
    {
        public TrimBlockStart() : base("trim_block_start") { }

        public override HandlebarsBlockHelper Helper =>
           (TextWriter output, HelperOptions options, object context, object[] arguments) =>
           {

               var trimChar = GetArgumentCharValueOrDefault(arguments, 0, ' ');

               TrimBlockHelper.Trim(output, options, context, (str) => str.TrimStart(trimChar));

           };

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
    public class TrimBlockEnd : SimpleBlockHelperBase
    {
        public TrimBlockEnd() : base("trim_block_end") { }

        public override HandlebarsBlockHelper Helper =>
           (TextWriter output, HelperOptions options, object context, object[] arguments) =>
           {
               var trimChar = GetArgumentCharValueOrDefault(arguments, 0, ' ');

               TrimBlockHelper.Trim(output, options, context, (str) => str.TrimEnd(trimChar));

           };

    }

    internal static class TrimBlockHelper
    {
        public static void Trim(TextWriter output, HelperOptions options, object context, Func<string, string> trimFunc)
        {

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
                    output.WriteSafeString(trimFunc(result));
                }
            }
        }
    }
}
