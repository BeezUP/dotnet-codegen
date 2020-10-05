//using DiffPlex;
//using DiffPlex.DiffBuilder;
//using DiffPlex.DiffBuilder.Model;
//using Newtonsoft.Json.Linq;
//using Shouldly;
//using System;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;
//using Xunit.Abstractions;

//namespace DocumentRefLoader.Tests
//{
//    public class ReferenceLoaderTests_OpenApiV2Merge
//    {
//        private readonly ITestOutputHelper _output;

//        public ReferenceLoaderTests_OpenApiV2Merge(ITestOutputHelper output)
//        {
//            _output = output;
//        }

//        [Theory]
//        //[InlineData("Merge1.yaml", "Merge1_expected.yaml")]
//        [InlineData("Merge2A.yaml", "Merge2_expected.yaml")]
//        public async Task Should_match_expected(string file, string expected)
//        {
//            var sut = new ReferenceLoader("./_yamlSamples/" + file, ReferenceLoaderStrategy.OpenApiV2Merge);
//            var yaml = await sut.GetRefResolvedYamlAsync();
//            yaml.InvariantNewline().ShouldBe(File.ReadAllText("./_yamlSamples/" + expected).InvariantNewline());
//        }

//        [Theory]
//        [InlineData("https://api.swaggerhub.com/apis/BeezUP/backends/1.0", "./_yamlSamples/BeezUP_backends_expected.yaml")]
//        public async Task Should_match_expected_online(string uri, string expected)
//        {
//            var sut = new ReferenceLoader(uri, ReferenceLoaderStrategy.OpenApiV2Merge);
//            var yaml = await sut.GetRefResolvedYamlAsync();
//            DumpFiles(sut);
//            yaml.InvariantNewline().ShouldBe(File.ReadAllText(expected).InvariantNewline());
//        }

//        public static void DumpFiles(ReferenceLoader refLoader)
//        {
//            var tmpFolder = "_tmp";
//            Directory.CreateDirectory(tmpFolder);
//            foreach (var kv in refLoader._loaders)
//            {
//                var fileName = kv.Key.ToString().Replace("/", "_").Replace(":", "");
//                File.WriteAllText(Path.Combine(tmpFolder, fileName + "_before"), kv.Value._originalJson);
//                File.WriteAllText(Path.Combine(tmpFolder, fileName + "_after"), kv.Value._finalJson);
//            }
//        }


//        void DumpDifferences3(ReferenceLoader refLoader)
//        {


//            foreach (var kv in refLoader._loaders)
//            {
//                _output.WriteLine("=====================================================================================");
//                _output.WriteLine("=====================================================================================");
//                _output.WriteLine($"\n{kv.Key}\n");

//                var rl = kv.Value;

//                var diffBuilder = new InlineDiffBuilder(new Differ());
//                var diff = diffBuilder.BuildDiffModel(rl._originalJson.InvariantNewline(), rl._finalJson.InvariantNewline());

//                foreach (var line in diff.Lines)
//                {
//                    switch (line.Type)
//                    {
//                        case ChangeType.Inserted:
//                            _output.WriteLine("+ " + line.Text);
//                            break;
//                        case ChangeType.Deleted:
//                            _output.WriteLine("- " + line.Text);
//                            break;
//                    }
//                }
//            }
//        }

//        void DumpDifferences2(ReferenceLoader refLoader)
//        {
//            foreach (var kv in refLoader._loaders)
//            {
//                _output.WriteLine("=====================================================================================");
//                _output.WriteLine("=====================================================================================");
//                _output.WriteLine($"\n{kv.Key}\n");

//                var rl = kv.Value;
//                var leftLines = rl._originalJson.InvariantNewline().Split("\n");
//                var rightLines = rl._finalJson.InvariantNewline().Split("\n");
//                var diff = DiffLib.Diff.CalculateSections(leftLines, rightLines);

//                var left = 0;
//                var right = 0;
//                foreach (var section in diff)
//                {
//                    if (section.LengthInCollection1 != 0 || section.LengthInCollection2 != 0)
//                    {
//                        var lefties = leftLines.Skip(left).Take(section.LengthInCollection1).ToArray();
//                        var righties = rightLines.Skip(right).Take(section.LengthInCollection2).ToArray();
//                        if (string.Join(Environment.NewLine, lefties) == string.Join(Environment.NewLine, righties)) continue;

//                        _output.WriteLine("");
//                        _output.WriteLine($"===================================================================================== lines left:{left} / right:{right}");

//                        string removed = string.Join(Environment.NewLine, lefties.Select(s => $"- {s}"));
//                        string added = string.Join(Environment.NewLine, righties.Select(s => $"+ {s}"));
//                        if (section.LengthInCollection1 != 0)
//                        {
//                            _output.WriteLine("");

//                            _output.WriteLine(removed);
//                        }
//                        if (section.LengthInCollection2 != 0)
//                        {
//                            _output.WriteLine("");
//                            _output.WriteLine(added);
//                        }
//                    }

//                    left += section.LengthInCollection1;
//                    right += section.LengthInCollection2;
//                }

//            }
//        }

//        void DumpJsonDifferences(ReferenceLoader refLoader)
//        {
//            foreach (var kv in refLoader._loaders)
//            {
//                var rl = kv.Value;

//                var jdp = new JsonDiffPatchDotNet.JsonDiffPatch();
//                var patch = jdp.Diff(JToken.Parse(rl._originalJson), JToken.Parse(rl._finalJson));
//                _output.WriteLine($"\n\n\n{kv.Key}\n\n");
//                _output.WriteLine(patch.ToString());
//            }
//        }
//    }
//}
