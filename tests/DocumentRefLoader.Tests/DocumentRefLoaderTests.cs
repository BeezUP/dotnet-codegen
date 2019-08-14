using DocumentRefLoader;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace DocumentRefLoader.Tests
{
    public class DocumentRefLoaderTests
    {
        private readonly ITestOutputHelper _output;

        public DocumentRefLoaderTests(ITestOutputHelper output)
        {
            _output = output;
        }

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
            var sut = new ReferenceLoader(document);

            var refInfo = sut.GetRefInfo(@ref);

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

        [Fact]
        public void Should_resolve_nested_references()
        {
            var sut = new ReferenceLoader("./_yamlSamples/petshop.yaml");
            {
                var yaml = sut.GetRefResolvedYaml();
                _output.WriteLine(yaml);
                yaml.Contains(ReferenceLoader.REF_KEYWORD).ShouldBeFalse();
            }
            {
                var json = sut.GetRefResolvedJson();
                _output.WriteLine(json);
                json.Contains(ReferenceLoader.REF_KEYWORD).ShouldBeFalse();
            }
        }

        [Fact]
        public void Should_resolve_external_references()
        {
            var sut = new ReferenceLoader("./_yamlSamples/petshop_with_external.yaml");
            {
                var yaml = sut.GetRefResolvedYaml();
                _output.WriteLine(yaml);
                yaml.Contains(ReferenceLoader.REF_KEYWORD).ShouldBeFalse();
            }
            {
                var json = sut.GetRefResolvedJson();
                _output.WriteLine(json);
                json.Contains(ReferenceLoader.REF_KEYWORD).ShouldBeFalse();
            }
        }

        [Theory]
        [MemberData(nameof(ResolveNested_Data))]
        public void ResolveNested(string document, string expectedYaml)
        {
            var fileName = Path.GetTempFileName();

            try
            {
                File.WriteAllText(fileName, document);

                var sut = new ReferenceLoader(fileName);
                var yaml = sut.GetRefResolvedYaml();

                yaml.InvariantNewline().ShouldBe(expectedYaml.InvariantNewline());
            }
            finally
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
        }

        public static IEnumerable<object[]> ResolveNested_Data()
        {
            yield return new[]
            {
@"test:
  $ref: '#/myref'
myref:
  data: test
",
@"test:
  data: test
myref:
  data: test
"
            };
            yield return new[]
            {
@"test:
  data1:
    $ref: '#/myref'
  data2:
    $ref: '#/myref'
myref:
  data: test
",
@"test:
  data1:
    data: test
  data2:
    data: test
myref:
  data: test
"
            };
            yield return new[]
            {
@"test:
  data1:
    $ref: '#/myref'
  data2:
    $ref: '#/myref'
myref:
  data:
    $ref: '#/myref2'
myref2:
  content:
    data: test
",
@"test:
  data1:
    data:
      content:
        data: test
  data2:
    data:
      content:
        data: test
myref:
  data:
    content:
      data: test
myref2:
  content:
    data: test
"
            };
            yield return new[] // works with json
           {
@"{
   ""test"": {
      ""data1"": {
         ""$ref"": ""#/myref""
      },
      ""data2"": {
         ""$ref"": ""#/myref""
      }
   },
   ""myref"": {
      ""data"": {
         ""$ref"": ""#/myref2""
      }
   },
   ""myref2"": {
      ""content"": {
         ""data"": ""test""
      }
   }
}
",
@"test:
  data1:
    data:
      content:
        data: test
  data2:
    data:
      content:
        data: test
myref:
  data:
    content:
      data: test
myref2:
  content:
    data: test
"
            };
        }

        [Fact]
        public void VeryTrickyTest()
        {
            var sut = new ReferenceLoader("./_yamlSamples/simple1.yaml");
            var yaml = sut.GetRefResolvedYaml();

            yaml.InvariantNewline().ShouldBe(
@"test:
  this: will load multiple files
finalvalue:
  value: this is the real final value
value:
  subvalue:
    value: this is the real final value
".InvariantNewline());
        }
    }
}
