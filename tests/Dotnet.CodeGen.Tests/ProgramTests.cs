using Dotnet.CodeGen.CodeGen;
using Dotnet.CodeGen.Schemas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dotnet.CodeGen.Tests
{
    public class ProgramTests
    {
        //https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file
        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        //https://stackoverflow.com/questions/278439/creating-a-temporary-directory-in-windows
        static string GetTemporaryDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        [IgnoreOnLinuxTheory]
        [InlineData("./_samples/test1/schema.json", new[] { "./_samples/test1/template/test.hbs" }, "./_samples/test1/expected", TemplateDuplicationHandlingStrategy.KeepLast, SourceSchemaType.RawJson)]
        [InlineData("./_samples/test3/sample_swagger.json", new[] { "./_samples/test3/template1/path1.hbs", "./_samples/test3/template2/path2.hbs" }, "./_samples/test3/expected", TemplateDuplicationHandlingStrategy.KeepLast, SourceSchemaType.RawJson)]
        public async Task ShouldParseCommandLineArgumentsAndApplyTemplate(string source, string[] templates, string expectedDirectory, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy, SourceSchemaType sourceSchemaType)
        {

            var expectedMd5Output = Directory.GetFiles(expectedDirectory).Select(file => CalculateMD5(file)).ToList();

            var templateDuplicationHandlingStrategyOption = $" {templateDuplicationHandlingStrategy}";
            var sourceSchemaTypeOption = $" {sourceSchemaType}";

            var outputDirectory = GetTemporaryDirectory();

            try
            {
                var args = new[] { "-s", sourceSchemaTypeOption, "-d", templateDuplicationHandlingStrategyOption, source, outputDirectory }.Concat(templates).ToArray();

                await Program.Main(args);

                var actualMd5Output = Directory.GetFiles(outputDirectory).Select(file => CalculateMD5(file)).ToList();

                Assert.Equal(expectedMd5Output.Count, actualMd5Output.Count);

                foreach (var expectedMd5 in expectedMd5Output)
                {
                    Assert.Contains(expectedMd5, actualMd5Output);
                }

            }
            finally
            {
                Directory.Delete(outputDirectory, true);
            }

        }
    }
}
