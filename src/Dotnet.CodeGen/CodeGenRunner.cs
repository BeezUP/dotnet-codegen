using System.Collections.Generic;
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
        private static IEnumerable<TemplateInfos> ConsolidateTemplates(IGrouping<string,TemplateInfos>[] templateInfos, IGrouping<string, TemplateInfos>[] duplicateTemplateInfos, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy)
        {
            switch (templateDuplicationHandlingStrategy)
            {
                case TemplateDuplicationHandlingStrategy.Throw:
                    
                    throw new InvalidDataException("Possible template(s) duplication - please use a unique template name [" + duplicateTemplateInfos.Select(templateGroup => templateGroup.Key).Aggregate((template1, template2) => $"{template1} | {template2}" + "]"));
               
                case TemplateDuplicationHandlingStrategy.KeepFirst:

                    foreach (var templateGroup in templateInfos)
                    {
                        yield return templateGroup.First();
                    }

                    break;
                case TemplateDuplicationHandlingStrategy.KeepLast:

                    foreach (var templateGroup in templateInfos)
                    {
                        yield return templateGroup.Last();
                    }

                    break;
            }
        }

        public static IEnumerable<TemplateInfos> GetTemplates(List<string> templatesPaths, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy)
        {
            var templates = templatesPaths.SelectMany(templatePath => TemplateHelper.GetTemplates(templatePath, "*.hbs").Where(t => !t.FileName.StartsWith("_")))
                                          .ToArray();

            if (templates.Length == 0)
                throw new InvalidDataException("No template found in path(s) " + templatesPaths.Aggregate((path1, path2) => $"{path1} | {path2}"));

            var templateGroups = templates.GroupBy(template => template.FileName)
                                          .ToArray();

            var templatesGroupDuplicates = templateGroups.Where(group => group.Count() > 1).ToArray();

            if (templatesGroupDuplicates.Length == 0) return templates;


            return ConsolidateTemplates(templateGroups, templatesGroupDuplicates, templateDuplicationHandlingStrategy);
        }  

        public static Task RunAsync(string sourcePath, ISchemaLoader schemaLoader, string templatePath, string outputPath, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy = TemplateDuplicationHandlingStrategy.Throw)
        {
            return RunAsync(sourcePath, schemaLoader, new List<string>() { templatePath }, outputPath);
        }

        public static async Task RunAsync(string sourcePath, ISchemaLoader schemaLoader, List<string> templatesPaths, string outputPath, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy = TemplateDuplicationHandlingStrategy.Throw)
        {
            var jsonObject = schemaLoader.LoadSchema(sourcePath);

            var templates = GetTemplates(templatesPaths, templateDuplicationHandlingStrategy);

            var handlebars = HandlebarsConfigurationHelper.GetHandlebars(templatesPaths);

            foreach (var template in templates)
            {
                var compiled = handlebars.Compile(File.ReadAllText(template.FilePath));

                var result = compiled(jsonObject);

                var context = new ProcessorContext { InputFile = result, OutputDirectory = outputPath, };

                using var processor = new FilesProcessor(context,
                    new WriteLineToFileInstruction("FILE"),
                    new WriteLineToConsoleInstruction("CONSOLE"),
                    new SuppressLineInstruction()
                    );

                await processor.RunAsync();
            }
        }
    }
}
