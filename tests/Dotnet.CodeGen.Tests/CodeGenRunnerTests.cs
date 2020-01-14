using Dotnet.CodeGen.CodeGen;
using Shouldly;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dotnet.CodeGen.Tests
{
    public class CodeGenRunnerTests
    {
        private readonly ITestOutputHelper _output;

        public CodeGenRunnerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(SourceSchemaType.RawJson, "./_samples/test3/sample_swagger.json", new[] { "./_samples/test3/template1/path1.hbs", "./_samples/test3/template2/path2.hbs" }, "./_samples/test3/expected")]
        [InlineData(SourceSchemaType.RawJson, "./_samples/test3/sample_swagger.json", new[] { "./_samples/test3/template1/path1.hbs", "./_samples/test3/template2" }, "./_samples/test3/expected")]
        [InlineData(SourceSchemaType.RawJson, "./_samples/test3/sample_swagger.json", new[] { "./_samples/test3/template1", "./_samples/test3/template2" }, "./_samples/test3/expected")]
        [InlineData(SourceSchemaType.RawJson, "./_samples/test1/schema.json", new[] { "./_samples/test1/template" }, "./_samples/test1/expected")]
        public async Task ShouldGetAndApplyOpenApiTemplates(SourceSchemaType schemaType, string schemaPath, string[] templatesPaths, string expectedResultPath)
        {
            var tmpFolder = Path.GetRandomFileName();
            try
            {
                var loader = schemaType.GetSchemaLoader();

                await CodeGenRunner.RunAsync(schemaPath, loader, templatesPaths.ToList(), tmpFolder);

                var expectedFiles = Directory.GetFiles(expectedResultPath, "*.*", SearchOption.AllDirectories).ToArray();
                var resultFiles = Directory.GetFiles(tmpFolder, "*.*", SearchOption.AllDirectories).ToArray();

                resultFiles
                    .Select(f => Path.GetRelativePath(tmpFolder, f)).OrderBy(f => f)
                    .ShouldBe(expectedFiles.Select(f => Path.GetRelativePath(expectedResultPath, f)).OrderBy(f => f));

                foreach (var expectedFile in expectedFiles)
                {
                    var relativePath = Path.GetRelativePath(expectedResultPath, expectedFile);
                    var resultPath = Path.Combine(tmpFolder, relativePath);

                    if (!File.Exists(resultPath))
                        throw new FileNotFoundException($"File {relativePath} should be output from the template runner.");

                    _output.WriteLine($"Testing {relativePath} content.");

                    File.ReadAllText(resultPath).Replace("\r\n", "\n")
                        .ShouldBe(File.ReadAllText(expectedFile).Replace("\r\n", "\n"));
                }
            }
            finally
            {
                if (Directory.Exists(tmpFolder))
                    Directory.Delete(tmpFolder, true);
            }
        }

        [Theory]
        [InlineData(new[] { "./_samples/test4/template1", "./_samples/test4/template2" }, TemplateDuplicationHandlingStrategy.Throw, "Possible template(s) duplication - please use a unique template name [path1 | path2 | path5]")]
        [InlineData(new[] { "./_samples/test4/template3", "./_samples/test4/template4" }, TemplateDuplicationHandlingStrategy.Throw, "Possible template(s) duplication - please use a unique template name [path3 | path4]")]
        [InlineData(new[] { "./_samples/test4/template1", "./_samples/test4/template2", "./_samples/test4/template2.2" }, TemplateDuplicationHandlingStrategy.Throw, "Possible template(s) duplication - please use a unique template name [path1 | path2 | path5]")]
        public void ShouldThrowExceptionOnConsolidateTemplates(string[] templatesPaths, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy, string expectedMessage)
        {
            Should.Throw<InvalidDataException>(() =>
            {
                CodeGenRunner.GetTemplates(templatesPaths.ToList(), templateDuplicationHandlingStrategy).ToList();
            }
            ).Message.ShouldBe(expectedMessage);
        }

        [Theory]
        [InlineData(new[] { "./_samples/test4/template1", "./_samples/test4/template3" }, TemplateDuplicationHandlingStrategy.Throw)]
        [InlineData(new[] { "./_samples/test4/template2", "./_samples/test4/template4" }, TemplateDuplicationHandlingStrategy.Throw)]
        public void ShouldNotThrowExceptionOnConsolidateTemplates(string[] templatesPaths, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy)
        {
            var templates = CodeGenRunner.GetTemplates(templatesPaths.ToList(), templateDuplicationHandlingStrategy).ToList();

            Assert.Equal(5, templates.Count);

            Assert.Contains(templates, template => template.FileName == "path1");
            Assert.Contains(templates, template => template.FileName == "path2");
            Assert.Contains(templates, template => template.FileName == "path3");
            Assert.Contains(templates, template => template.FileName == "path4");
        }

        [Theory]
        [InlineData(new[] { "./_samples/test4/template1", "./_samples/test4/template2" }, "template1", TemplateDuplicationHandlingStrategy.KeepFirst, 3)]
        [InlineData(new[] { "./_samples/test4/template3", "./_samples/test4/template4" }, "template3", TemplateDuplicationHandlingStrategy.KeepFirst, 2)]
        [InlineData(new[] { "./_samples/test4/template1", "./_samples/test4/template2" }, "template2", TemplateDuplicationHandlingStrategy.KeepLast, 3)]
        [InlineData(new[] { "./_samples/test4/template3", "./_samples/test4/template4" }, "template4", TemplateDuplicationHandlingStrategy.KeepLast, 2)]
        public void ShouldKeepOnConsolidateTemplates(string[] templatesPaths, string expectedTemplateFolder, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy, int expectedTemplatesCount)
        {
            var templates = CodeGenRunner.GetTemplates(templatesPaths.ToList(), templateDuplicationHandlingStrategy).ToList();

            templates.Count.ShouldBe(expectedTemplatesCount);

            foreach (var template in templates)
            {
                Assert.Contains(expectedTemplateFolder, template.FilePath);
            }
        }
    }
}
