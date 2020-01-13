using Shouldly;
using System;
using System.IO;
using Xunit;

namespace DocumentRefLoader.Tests
{
    public class RefInfoTests
    {
        [Theory]
        [InlineData("_yamlSamples/petshop.yaml", "../test.json", false, true, "test.json", "")]
        [InlineData("_yamlSamples/petshop.yaml", "test.json", false, true, "_yamlSamples/test.json", "")]
        [InlineData("_yamlSamples/petshop.yaml", "#test", true, true, "_yamlSamples/petshop.yaml", "test")]
        [InlineData("_yamlSamples/petshop.yaml", "test.json#test", false, true, "_yamlSamples/test.json", "test")]
        [InlineData("_yamlSamples/petshop.yaml", "http://google.com/test.json#test", false, false, "http://google.com/test.json", "test")]
        [InlineData("https://petstore.swagger.io/v2/swagger.json", "#/definitions/Pet", true, false, "https://petstore.swagger.io/v2/swagger.json", "/definitions/Pet")]
        [InlineData("https://petstore.swagger.io/v2/swagger.json", "http://google.com/test.json#test", false, false, "http://google.com/test.json", "test")]
        [InlineData("https://petstore.swagger.io/v2/swagger.json", "http://google.com/test.json", false, false, "http://google.com/test.json", "")]
        [InlineData("https://petstore.swagger.io/v2/swagger.json", "../test.json", false, false, "https://petstore.swagger.io/test.json", "")]
        [InlineData("https://petstore.swagger.io/v2/swagger.json", "../test.json#fragment", false, false, "https://petstore.swagger.io/test.json", "fragment")]
        public void GetRefInfo(string document, string @ref, bool expectedIsNested, bool expectedIsLocal, string expectedUri, string expectedPath)
        {
            var refInfo = RefInfo.GetRefInfo(document, @ref);

            if (!new Uri(expectedUri, UriKind.RelativeOrAbsolute).IsAbsoluteUri)
            {
                expectedUri = Path.Combine(Directory.GetCurrentDirectory(), expectedUri);
                expectedUri = new Uri(Path.GetFullPath(expectedUri)).AbsoluteUri.ToString();
            }
            else
            {
                expectedUri = new Uri(expectedUri).AbsoluteUri.ToString();
            }

            var absoluteUri = refInfo.AbsoluteDocumentUri.AbsoluteUri.ToString();

            refInfo.IsNestedInThisDocument.ShouldBe(expectedIsNested);
            refInfo.IsLocal.ShouldBe(expectedIsLocal);
            absoluteUri.ShouldBe(expectedUri);
            refInfo.InDocumentPath.ShouldBe(expectedPath);
        }
    }
}
