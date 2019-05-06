using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotnet.CodeGen.CodeGen.Instructions;
using Dotnet.CodeGen.CustomHandlebars;
using Dotnet.CodeGen.Schemas;

namespace Dotnet.CodeGen.CodeGen
{
    public class CodeGenRunner
    {
        public static async Task RunAsync(string sourcePath, ISchemaLoader schemaLoader, string templatePath, string outputPath)
        {
            var jsonObject = schemaLoader.LoadSchema(sourcePath);

            var templates = TemplateHelper.GetTemplates(templatePath, "*.hbs")
                .Where(t => !t.FileName.StartsWith("_"))
                .ToArray();

            if (templatePath.Length == 0)
                throw new InvalidDataException($"No template found in {templatePath}.");

            var handlebars = HandlebarsConfigurationHelper.GetHandlebars(templatePath);

            foreach (var template in templates)
            {
                var compiled = handlebars.Compile(File.ReadAllText(template.FilePath));

                var result = compiled(jsonObject);

                var context = new ProcessorContext { InputFile = result, OutputDirectory = outputPath, };

                using (var processor = new FilesProcessor(context,
                    new WriteLineToFileInstruction("FILE"),
                    new WriteLineToConsoleInstruction("CONSOLE"),
                    new SuppressLineInstruction()
                    ))
                {
                    await processor.RunAsync();
                }
            }
        }
    }
}
