using Dotnet.CodeGen.CodeGen.CustomHandlebars;
using Shouldly;
using System.IO;
using System.Text;
using Xunit;

namespace BeezUP2.Framework.CodeGen.Tests.Helpers.UppercaseFirstLetterHelper
{
    public class CodeGenHandlebarsHelpersTests
    {
        [Theory]
        [InlineData("hello !", "Hello !")]
        [InlineData("Hello !", "Hello !")]
        [InlineData("", "")]
        [InlineData("a", "A")]
        [InlineData("â", "Â")]
        public void UppercaseFirstLetterHelper(string input, string expected)
        {
            var builder = new StringBuilder();

            using (var writer = new StringWriter(builder))
            {
                CodeGenHandlebarsHelpers.UppercaseFirstLetterHelper(writer, null, input);
            }

            var output = builder.ToString();
            output.ShouldBe(expected);
        }
    }
}
