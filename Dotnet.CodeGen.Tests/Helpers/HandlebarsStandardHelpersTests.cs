using Dotnet.CodeGen.CustomHandlebars;
using Dotnet.CodeGen.CustomHandlebars.Standard;
using Shouldly;
using System.IO;
using System.Text;
using Xunit;

namespace Dotnet.CodeGen.Tests.Helpers.UppercaseFirstLetterHelper
{
    public class HandlebarsStandardHelpersTests
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
                new UppercaseFirstLetter().Helper(writer, null, input);
            }

            var output = builder.ToString();
            output.ShouldBe(expected);
        }
    }
}
