using CodegenUP.CodeGen;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CodegenUP.Tests
{
    public class ProgramTests
    {
        //https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file
        /// <summary>
        /// Compute MD5 for a file, after replace replacing windows new lines
        /// </summary>
        static string ComputeMD5(string filename)
        {
            var fileContent = File.ReadAllText(filename).InvariantNewline();
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(fileContent);
                var hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
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
            var expectedMd5Output = Directory.GetFiles(expectedDirectory).Select(file => ComputeMD5(file)).ToList();

            var outputDirectory = GetTemporaryDirectory();

            try
            {
                var args = new[] {
                    "-l", sourceSchemaType.ToString(),
                    "-d", templateDuplicationHandlingStrategy.ToString(),
                    "-s", source,
                    "-o", outputDirectory
                    }
                    .Concat(templates.SelectMany(t => new[] { "-t", t }))
                    .ToArray();

                await Program.Main(args);

                var actualMd5Output = Directory.GetFiles(outputDirectory).Select(file => ComputeMD5(file)).ToList();

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
