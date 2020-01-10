﻿using System;
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
    public static class CodeGenRunner
    {
        public static IEnumerable<TemplateInfos> GetTemplates(List<string> templatesPaths, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy)
        {
            var templates = templatesPaths
                .SelectMany(templatePath => TemplateHelper.GetTemplates(templatePath, "*.hbs"))
                .Where(t => !t.FileName.StartsWith("_"))
                .ToArray();

            if (templates.Length == 0)
                throw new InvalidDataException($"No template found in path(s) : {string.Join(" | ", templatesPaths)}");

            var templateGroups = templates.GroupBy(template => template.FileName).ToArray();

            if (templateDuplicationHandlingStrategy == TemplateDuplicationHandlingStrategy.Throw)
            {
                var duplicates = templateGroups.Where(group => group.Count() > 1).ToArray();
                if (duplicates.Length != 0)
                {
                    var templateNames = string.Join(" | ", duplicates.Select(g => g.Key).OrderBy(template => template));
                    throw new InvalidDataException($"Possible template(s) duplication - please use a unique template name [{templateNames}]");
                }
            }

            return templateGroups.Select(g =>
            {
                switch (templateDuplicationHandlingStrategy)
                {
                    case TemplateDuplicationHandlingStrategy.Throw: // should be only one element in the group now
                    case TemplateDuplicationHandlingStrategy.KeepFirst:
                        return g.First();
                    case TemplateDuplicationHandlingStrategy.KeepLast:
                        return g.Last();
                    default:
                        throw new NotImplementedException();
                }
            });
        }

        public static Task RunAsync(string sourcePath, ISchemaLoader schemaLoader, string templatePath, string outputPath, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy = TemplateDuplicationHandlingStrategy.Throw)
            => RunAsync(sourcePath, schemaLoader, new List<string>() { templatePath }, outputPath, templateDuplicationHandlingStrategy);

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
