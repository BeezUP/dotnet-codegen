using DiffLib;
using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace DocumentRefLoader.Tests
{
    public class ReferenceLoaderTests_OpenApiV2Merge
    {
        private readonly ITestOutputHelper _output;

        public ReferenceLoaderTests_OpenApiV2Merge(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("Merge1.yaml", "Merge1_expected.yaml")]
        public void Should_match_expected(string file, string expected)
        {
            var sut = new ReferenceLoader("./_yamlSamples/" + file, ReferenceLoaderStrategy.OpenApiV2Merge);
            {
                var yaml = sut.GetRefResolvedYaml();
                yaml.InvariantNewline().ShouldBe(File.ReadAllText("./_yamlSamples/" + expected).InvariantNewline());
            }
        }

        [Theory]
        [InlineData("https://api.swaggerhub.com/apis/BeezUP/backends/1.0", "./_yamlSamples/BeezUP_backends_expected.yaml")]
        public void Should_match_expected_online(string uri, string expected)
        {
            var sut = new ReferenceLoader(uri, ReferenceLoaderStrategy.OpenApiV2Merge);
            {
                var yaml = sut.GetRefResolvedYaml();
                DumpDifferences(sut);
                yaml.InvariantNewline().ShouldBe(File.ReadAllText(expected).InvariantNewline());
            }
        }

        public void DumpDifferences(ReferenceLoader refLoader)
        {
            foreach (var kv in refLoader._otherLoaders)
            {
                _output.WriteLine("====================================================================================");
                _output.WriteLine("====================================================================================");
                _output.WriteLine($"\n{kv.Key}\n");
                _output.WriteLine("====================================================================================");
                _output.WriteLine("");

                var rl = kv.Value;
                var leftLines = rl.OriginalJson.InvariantNewline().Split("\n");
                var rightLines = rl.FinalJson.InvariantNewline().Split("\n");
                var diff = Diff.CalculateSections(leftLines, rightLines);

                var left = 0;
                var right = 0;
                foreach (var section in diff)
                {
                    if (section.IsMatch)
                    {
                        //_output.WriteLine($"==================================================================================== line {left}, identical:");
                        //_output.WriteLine(string.Join(Environment.NewLine, leftLines.Skip(left).Take(section.LengthInCollection1)));
                    }
                    else
                    {
                        if (section.LengthInCollection1 != 0)
                        {
                            _output.WriteLine($"==================================================================================== line {left}, removed:");
                            _output.WriteLine(string.Join(Environment.NewLine, leftLines.Skip(left).Take(section.LengthInCollection1).Select(s => $"- {s}")));
                            _output.WriteLine("");
                        }
                        if (section.LengthInCollection2 != 0)
                        {
                            _output.WriteLine($"==================================================================================== line {right}, added:");
                            _output.WriteLine(string.Join(Environment.NewLine, leftLines.Skip(right).Take(section.LengthInCollection2).Select(s => $"+ {s}")));
                            _output.WriteLine("");
                        }
                    }

                    left += section.LengthInCollection1;
                    right += section.LengthInCollection2;
                }

            }
        }

        public void DumpJsonDifferences(ReferenceLoader refLoader)
        {
            foreach (var kv in refLoader._otherLoaders)
            {
                var rl = kv.Value;

                var jdp = new JsonDiffPatch();
                var patch = jdp.Diff(JToken.Parse(rl.OriginalJson), JToken.Parse(rl.FinalJson));
                _output.WriteLine($"\n\n\n{kv.Key}\n\n");
                _output.WriteLine(patch.ToString());
            }
        }
    }
}
