using HandlebarsDotNet;
using System.IO;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// Uppercase the first letter
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{uppercase_first_letter .}}", "{}")]
    [HandlebarsHelperSpecification("{ test: 42 }", "{{uppercase_first_letter test}}", "42")]
    [HandlebarsHelperSpecification("{ test: '42' }", "{{uppercase_first_letter test}}", "42")]
    [HandlebarsHelperSpecification("{ test: 'aa' }", "{{uppercase_first_letter test}}", "Aa")]
    [HandlebarsHelperSpecification("{ test: 'AA' }", "{{uppercase_first_letter test}}", "AA")]
    [HandlebarsHelperSpecification("{ test: 'AA' }", "test{{uppercase_first_letter test}}", "testAA")]
#endif
    public class UppercaseFirstLetter : SimpleStandardHelperBase<object, string>
    {
        public UppercaseFirstLetter() : base("uppercase_first_letter") { }

        public override void HelperFunction(TextWriter output, object context, string argument, object[] arguments)
        {
            output.Write(StringUppercaseFirstLetter(argument));
        }

        public static string StringUppercaseFirstLetter(string argument)
            => string.IsNullOrEmpty(argument)
                ? string.Empty
                : argument.Length == 1
                    ? argument.ToUpper()
                    : argument.Substring(0, 1).ToUpper() + argument.Substring(1);
    }
}
