using Dotnet.CodeGen.CustomHandlebars;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

#if DEBUG

namespace Dotnet.CodeGen.Tests
{
    public class PrintoutHelpersList
    {
        private readonly ITestOutputHelper _output;

        public PrintoutHelpersList(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <summary>
        /// This test is just a way to generate a documentation for existing custom helpers
        /// </summary>
        [Fact]
        public void PrintDocs()
        {
            string escapeStr(string str)
            {
                return str.Replace("\r", "\\r").Replace("\n", "\\n");
            };

            foreach (var helper in HandlebarsConfigurationHelper.DefaultHelpers.OrderBy(h => h.ToString()))
            {
                _output.WriteLine($"### {helper}");

                _output.WriteLine("| Input document | Handlebars template | Result |");
                _output.WriteLine("|----------------|---------------------|--------|");

                foreach (var att in helper.GetType().GetCustomAttributes(typeof(HandlebarsHelperSpecificationAttribute), false).Cast<HandlebarsHelperSpecificationAttribute>())
                {
                    const int jsonLimit = 100;
                    var jsonDoc = att.GetJsonDocument();
                    var json = jsonDoc.Length >= jsonLimit
                        ? jsonDoc.Substring(0, jsonLimit) + "..."
                        : jsonDoc
                        ;

                    _output.WriteLine($"| `{escapeStr(json)}` | `{escapeStr(att.Template)}` | `{escapeStr(att.ExpectedOutput)}` |");
                }
            }
        }
    }
}

#endif