using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dotnet.CodeGen.CustomHandlebars.Block;
using HandlebarsDotNet;

namespace Dotnet.CodeGen.CustomHandlebars.Standard
{
    /// <summary>
    /// Trim Start & End
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{ test: 42 }", "{{trim test}}", "42")]
    [HandlebarsHelperSpecification("{ test: ' 42 ' }", "{{trim test}}", "42")]
    [HandlebarsHelperSpecification("{ test: '- aa -' }", "{{trim test '-'}}", " aa ")]
    [HandlebarsHelperSpecification("{ test: 'AA' }", "{{trim test 'A'}}", "")]
    [HandlebarsHelperSpecification("{ test: ' test ' }", "{{trim test ' t'}}", "es")]
#endif
    public class Trim : StandardHelperBase
    {
        public Trim() : base("trim") { }

        public override HandlebarsHelper Helper =>
            (TextWriter output, object context, object[] arguments) =>
            {
                EnsureArgumentsCountMin(arguments, 1);
                EnsureArgumentsCountMax(arguments, 2);

                TrimHelper.Trim(output, arguments, (a, c) => a.Trim(c));
            };
    }

    /// <summary>
    /// Trim Start
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{ test: 42 }", "{{trim_start test}}", "42")]
    [HandlebarsHelperSpecification("{ test: ' 42' }", "{{trim_start test}}", "42")]
    [HandlebarsHelperSpecification("{ test: '- aa' }", "{{trim_start test '-'}}", " aa")]
    [HandlebarsHelperSpecification("{ test: 'AA' }", "{{trim_start test 'A'}}", "")]
    [HandlebarsHelperSpecification("{ test: ' test ' }", "{{trim_start test ' t'}}", "est ")]
#endif
    public class TrimStart : StandardHelperBase
    {
        public TrimStart() : base("trim_start") { }

        public override HandlebarsHelper Helper =>
            (TextWriter output, object context, object[] arguments) =>
            {
                EnsureArgumentsCountMin(arguments, 1);
                EnsureArgumentsCountMax(arguments, 2);

                TrimHelper.Trim(output, arguments, (a, c) => a.TrimStart(c));
            };
    }

    /// <summary>
    /// Trim End
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{ test: 42 }", "{{trim_end test}}", "42")]
    [HandlebarsHelperSpecification("{ test: '42 ' }", "{{trim_end test}}", "42")]
    [HandlebarsHelperSpecification("{ test: 'aa -' }", "{{trim_end test '-'}}", "aa ")]
    [HandlebarsHelperSpecification("{ test: 'AA' }", "{{trim_end test 'A'}}", "")]
    [HandlebarsHelperSpecification("{ test: ' test ' }", "{{trim_end test ' t'}}", " tes")]
#endif
    public class TrimEnd : StandardHelperBase
    {
        public TrimEnd() : base("trim_end") { }

        public override HandlebarsHelper Helper =>
            (TextWriter output, object context, object[] arguments) =>
            {
                EnsureArgumentsCountMin(arguments, 1);
                EnsureArgumentsCountMax(arguments, 2);

                TrimHelper.Trim(output, arguments, (a, c) => a.TrimEnd(c));
            };
    }


    internal static class TrimHelper
    {
        public static void Trim(TextWriter output, object[] arguments, Func<string, char[], string> trimFunc)
        {
            var argument = arguments[0].ToString();
            var trimChars = arguments.Length == 2 ? arguments[1].ToString().ToArray() : new[] { ' ' };

            if (string.IsNullOrEmpty(argument))
            {
                output.Write(string.Empty);
            }
            else
            {
                output.Write(trimFunc(argument, trimChars));
            }
        }
    }
}
