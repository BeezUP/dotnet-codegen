using HandlebarsDotNet;
using System;
using System.IO;
using System.Linq;

namespace CodegenUP.CustomHandlebars.Helpers
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
    public class Trim : SimpleStandardHelperBase<object, string>
    {
        public Trim() : base("trim") { }
        protected Trim(string name) : base(name) { }

        public override void HelperFunction(TextWriter output, object context, string toTrim, object[] otherArguments)
        {
            EnsureArgumentsCountMax(otherArguments, 1); // todo :: this argument position is false because the `otherAguments` are skiped from already there args

            if (!string.IsNullOrEmpty(toTrim))
            {
                var trimChars = TryGetArgumentAsString(otherArguments, 0, out var str)
                    ? str.ToArray()
                    : new[] { ' ' };

                output.Write(TrimStr(toTrim, trimChars));
            }
        }

        protected virtual string TrimStr(string toTrim, char[] trimChars) => toTrim.Trim(trimChars);
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
    public class TrimStart : Trim
    {
        public TrimStart() : base("trim_start") { }
        protected override string TrimStr(string toTrim, char[] trimChars) => toTrim.TrimStart(trimChars);
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
    public class TrimEnd : Trim
    {
        public TrimEnd() : base("trim_end") { }
        protected override string TrimStr(string toTrim, char[] trimChars) => toTrim.TrimEnd(trimChars);
    }
}
