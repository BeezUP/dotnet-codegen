using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars.Standard
{
    [HandlebarsHelperSpecification("{}", "{{uppercase_first_letter {}}}", "")]
    [HandlebarsHelperSpecification("{ test: 42 }", "{{uppercase_first_letter test}}", "42")]
    [HandlebarsHelperSpecification("{ test: '42' }", "{{uppercase_first_letter test}}", "42")]
    [HandlebarsHelperSpecification("{ test: 'aa' }", "{{uppercase_first_letter test}}", "Aa")]
    [HandlebarsHelperSpecification("{ test: 'AA' }", "{{uppercase_first_letter test}}", "AA")]
    public class UppercaseFirstLetter : StandardHelperBase
    {
        public UppercaseFirstLetter() :
            base(
                "uppercase_first_letter",
                () => (TextWriter output, object context, object[] arguments) =>
                {
                    var argument = arguments.FirstOrDefault()?.ToString();

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
                })
        {
        }
    }
}
