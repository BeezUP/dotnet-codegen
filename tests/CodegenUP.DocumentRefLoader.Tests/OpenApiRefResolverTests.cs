//using Shouldly;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;

//namespace DocumentRefLoader.Tests
//{
//    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1199:Nested code blocks should not be used", Justification = "it's OK")]
//    public class ReferenceLoaderTests_RawCopy2
//    {
//        private readonly ITestOutputHelper _output;

//        public ReferenceLoaderTests_RawCopy2(ITestOutputHelper output)
//        {
//            _output = output;
//        }

//        [Fact]
//        public async Task Should_resolve_nested_references()
//        {
//            var sut = new OpenApiRefResolver("./_yamlSamples/petshop.yaml");
//            {
//                var yaml = await sut.GetRefResolvedYamlAsync();
//                _output.WriteLine(yaml);
//                yaml.Contains(Constants.REF_KEYWORD).ShouldBeFalse();
//            }
//            {
//                var json = await sut.GetRefResolvedJsonAsync();
//                _output.WriteLine(json);
//                json.Contains(Constants.REF_KEYWORD).ShouldBeFalse();
//            }
//        }

//        [Fact]
//        public async Task Should_resolve_external_references()
//        {
//            var sut = new OpenApiRefResolver("./_yamlSamples/petshop_with_external.yaml");
//            {
//                var yaml = await sut.GetRefResolvedYamlAsync();
//                _output.WriteLine(yaml);
//                yaml.Contains(Constants.REF_KEYWORD).ShouldBeFalse();
//            }
//            {
//                var json = await sut.GetRefResolvedJsonAsync();
//                _output.WriteLine(json);
//                json.Contains(Constants.REF_KEYWORD).ShouldBeFalse();
//            }
//        }

//        [Theory]
//        [MemberData(nameof(ResolveNested_Data))]
//        public void ResolveNested(string document, string expectedYaml)
//        {
//            var fileName = Path.GetTempFileName();

//            try
//            {
//                File.WriteAllText(fileName, document);

//                var sut = new OpenApiRefResolver(fileName);
//                var yaml = sut.GetRefResolvedYamlAsync().Result;

//                yaml.InvariantNewline().ShouldBe(expectedYaml.InvariantNewline());
//            }
//            finally
//            {
//                if (File.Exists(fileName))
//                    File.Delete(fileName);
//            }
//        }

//        public static IEnumerable<object[]> ResolveNested_Data()
//        {
//            yield return new[]
//            {
//@"test:
//  $ref: '#/myref'
//myref:
//  data: test
//",
//@"test:
//  data: ""test""
//myref:
//  data: ""test""
//"
//            };
//            yield return new[]
//            {
//@"test:
//  data1:
//    $ref: '#/myref'
//  data2:
//    $ref: '#/myref'
//myref:
//  data: test
//",
//@"test:
//  data1:
//    data: ""test""
//  data2:
//    data: ""test""
//myref:
//  data: ""test""
//"
//            };
//            yield return new[]
//            {
//@"test:
//  data1:
//    $ref: '#/myref'
//  data2:
//    $ref: '#/myref'
//myref:
//  data:
//    $ref: '#/myref2'
//myref2:
//  content:
//    data: test
//",
//@"test:
//  data1:
//    data:
//      content:
//        data: ""test""
//  data2:
//    data:
//      content:
//        data: ""test""
//myref:
//  data:
//    content:
//      data: ""test""
//myref2:
//  content:
//    data: ""test""
//"
//            };
//            yield return new[] // works with json
//           {
//@"{
//   ""test"": {
//      ""data1"": {
//         ""$ref"": ""#/myref""
//      },
//      ""data2"": {
//         ""$ref"": ""#/myref""
//      }
//   },
//   ""myref"": {
//      ""data"": {
//         ""$ref"": ""#/myref2""
//      }
//   },
//   ""myref2"": {
//      ""content"": {
//         ""data"": ""test""
//      }
//   }
//}
//",
//@"test:
//  data1:
//    data:
//      content:
//        data: ""test""
//  data2:
//    data:
//      content:
//        data: ""test""
//myref:
//  data:
//    content:
//      data: ""test""
//myref2:
//  content:
//    data: ""test""
//"
//            };
//        }

//        [Fact]
//        public void VeryTrickyTest()
//        {
//            var sut = new OpenApiRefResolver("./_yamlSamples/simple1.yaml");
//            var yaml = sut.GetRefResolvedYamlAsync().Result;

//            yaml.InvariantNewline().ShouldBe(
//@"test:
//  this: ""will load multiple files""
//finalvalue:
//  value: ""this is the real final value""
//value:
//  subvalue:
//    value: ""this is the real final value""
//".InvariantNewline());
//        }
//    }
//}
