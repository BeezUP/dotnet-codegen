using Dotnet.CodeGen.CodeGen;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        [InlineData(SourceSchemaType.RawJson, "./_samples/test1/schema.json", "./_samples/test1/template", "./_samples/test1/expected")]
        //[InlineData(SchemaType.OpenApi, "./_samples/exceptions/BeezUP-om_invoice-exception-1.0-domain.json", "./_samples/exceptions/template", "./_samples/exceptions/expected")]
        //[InlineData(SchemaType.OpenApi, "./_samples/enums/BeezUP-MKP_Feeds-schemas-1.0-domain.json", "./_samples/enums/template", "./_samples/enums/expected")]
        //[InlineData(SchemaType.OpenApi, "./_samples/controllers/manually_merged.json", "./_samples/controllers/template", "./_samples/controllers/expected")]
        public async Task OpenApiTemplates(SourceSchemaType schemaType, string schemaPath, string templatePath, string expectedResultPath)
        {
            var tmpFolder = Path.GetRandomFileName();
            try
            {
                var loader = schemaType.GetSchemaLoader();

                await CodeGenRunner.RunAsync(schemaPath, loader, templatePath, tmpFolder);

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
    }
}
