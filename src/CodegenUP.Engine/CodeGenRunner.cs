﻿using CodegenUP.Instructions;
using CodegenUP.CustomHandlebars;
using CodegenUP.Schemas;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodegenUP
{
    public static class CodeGenRunner
    {
        public static Task RunAsync(string sourcePath, ISchemaLoader schemaLoader, string templatePath, string outputPath, IEnumerable<IHelper>? customHelpers = null, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy = TemplateDuplicationHandlingStrategy.Throw, string? authorization = null)
            => RunAsync(new[] { sourcePath }, schemaLoader, new[] { templatePath }, outputPath, customHelpers, templateDuplicationHandlingStrategy, authorization);

        public static async Task RunAsync(IEnumerable<string> sourcePath, ISchemaLoader schemaLoader, IEnumerable<string> templatesPaths, string outputPath, IEnumerable<IHelper>? customHelpers, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy = TemplateDuplicationHandlingStrategy.Throw, string? authorization = null)
        {
            var jsonObject = await schemaLoader.LoadSchemaAsync(sourcePath, authorization);
            await RunAsync(jsonObject, templatesPaths, outputPath, customHelpers, templateDuplicationHandlingStrategy);
        }

        public static async Task RunAsync(JToken json, IEnumerable<string> templatesPaths, string outputPath, IEnumerable<IHelperBase>? helpers, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy = TemplateDuplicationHandlingStrategy.Throw)
        {
            helpers ??= new IHelper[0];

            var templates = GetTemplates(templatesPaths, templateDuplicationHandlingStrategy);

            var handlebars = HandlebarsConfigurationHelper.GetHandlebars(templatesPaths, helpers);

            foreach (var template in templates)
            {
                var compiled = handlebars.Compile(File.ReadAllText(template.FilePath));

                var result = compiled(json);

                var context = new ProcessorContext(result, outputPath);

                using var processor = new FilesProcessor(context,
                    new WriteLineToFileInstruction("FILE"),
                    new WriteLineToConsoleInstruction("CONSOLE"),
                    new CleanInstruction("CLEAN"),
                    new SuppressLineInstruction()
                    );

                await processor.RunAsync();
            }
        }

        internal static IEnumerable<TemplateInfos> GetTemplates(IEnumerable<string> templatesPaths, TemplateDuplicationHandlingStrategy templateDuplicationHandlingStrategy)
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
    }
}
