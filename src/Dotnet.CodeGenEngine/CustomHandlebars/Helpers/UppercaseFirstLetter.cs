using HandlebarsDotNet;
using System.IO;

namespace Dotnet.CodeGen.CustomHandlebars.Helpers
{
    /// <summary>
    /// Uppercase the first letter
    /// </summary>
#if DEBUG
    //[HandlebarsHelperSpecification("{}", "{{uppercase_first_letter .}}", "{}")] // unmeaningfull test working only with JObject
    [HandlebarsHelperSpecification("{ test: 42 }", "{{uppercase_first_letter test}}", "42")]
    [HandlebarsHelperSpecification("{ test: '42' }", "{{uppercase_first_letter test}}", "42")]
    [HandlebarsHelperSpecification("{ test: 'aa' }", "{{uppercase_first_letter test}}", "Aa")]
    [HandlebarsHelperSpecification("{ test: 'AA' }", "{{uppercase_first_letter test}}", "AA")]
    [HandlebarsHelperSpecification("{ test: 'AA' }", "test{{uppercase_first_letter test}}", "testAA")]
#endif
    public class UppercaseFirstLetter : SimpleStandardHelperBase
    {
        public UppercaseFirstLetter() : base("uppercase_first_letter") { }

        public override HandlebarsHelper Helper =>
            (TextWriter output, object context, object[] arguments) =>
                {
                    EnsureArgumentsCount(arguments, 1);

                    var argument = arguments[0].ToString();

                    if (string.IsNullOrEmpty(argument))
                    {
                        output.Write(string.Empty);
                    }
                    else
                    {
                        string res;
                        if (argument.Length == 1)
                        {
                            res = argument.ToUpper();
                        }
                        else
                        {
                            res = argument.Substring(0, 1).ToUpper() + argument.Substring(1);
                        }

                        output.Write(res);
                    }
                };
    }
}
