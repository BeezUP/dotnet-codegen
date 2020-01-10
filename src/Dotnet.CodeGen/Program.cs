using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Console = Colorful.Console;

namespace Dotnet.CodeGen.CodeGen
{
    public class Program
    {
        public static readonly Color ERROR_COLOR = Color.OrangeRed;

        public const string SOURCE_FILE_ARGUMENT = "source";
        public const string OUTPUT_PATH_ARGUMENT = "out";
        public const string TEMPLATES_PATH_ARGUMENT = "templates";
        
        public const string TEMPLATE_DUPLICATES_HANDLING_STRATEGY_OPTION = "-d|--duplicates";
        public const string SCHEMA_TYPE_OPTION = "-s|--type";
        private const string HelpOptions = "-? |-h|--help";

        public static async Task Main(string[] args)
        {
            //https://msdn.microsoft.com/fr-fr/magazine/mt763239.aspx
            var app = new CommandLineApplication();
            app.HelpOption(HelpOptions);

            var schemaTypeName = app.Option(SCHEMA_TYPE_OPTION, $"Enter a schema type between those values [{string.Join(" | ", Enum.GetNames(typeof(SourceSchemaType)))}]", CommandOptionType.SingleValue);
            var duplicatesTemplateHandlingStrategyName = app.Option(TEMPLATE_DUPLICATES_HANDLING_STRATEGY_OPTION, $"Enter a template duplication handling strategy between those values [{string.Join(" | ", Enum.GetNames(typeof(TemplateDuplicationHandlingStrategy)))}]", CommandOptionType.SingleValue);

            var sourceFile = app.Argument(SOURCE_FILE_ARGUMENT, "Enter the path (relative or absolute) to an source document.");
            var outputPath = app.Argument(OUTPUT_PATH_ARGUMENT, "Enter the path (relative or absolute) to the output path (content will be overritten)");
            var templatesPaths = app.Argument(TEMPLATES_PATH_ARGUMENT, "Enter the path(s) (relative or absolute / multiple files or folders) to the template(s).", multipleValues: true);

            app.OnExecute(async () =>
            {
                var errors = new List<string>();
                if (sourceFile.Value == null)
                    errors.Add($"{SOURCE_FILE_ARGUMENT} parameter is required");
                if (templatesPaths.Values == null || templatesPaths.Values.Count == 0)
                    errors.Add($"{TEMPLATES_PATH_ARGUMENT} parameter is required");
                if (outputPath.Value == null)
                    errors.Add($"{OUTPUT_PATH_ARGUMENT} parameter is required");

                var schemaType = SchemaTypeExtensions.DEFAULT_SHEMA_TYPE;
                if (schemaTypeName.HasValue())
                {
                    if (Enum.TryParse<SourceSchemaType>(schemaTypeName.Value(), out var schema))
                        schemaType = schema;
                    else
                        errors.Add($"The schema type '{schemaTypeName.Value()}' is unknown.");
                }

                var duplicatesTemplateHandlingStrategy = TemplateDuplicationHandlingStrategy.Throw;
                if (duplicatesTemplateHandlingStrategyName.HasValue())
                {
                    if (Enum.TryParse<TemplateDuplicationHandlingStrategy>(duplicatesTemplateHandlingStrategyName.Value(), out var strategy))
                        duplicatesTemplateHandlingStrategy = strategy;
                    else
                        errors.Add($"The duplicates template handling strategy type '{duplicatesTemplateHandlingStrategyName.Value()}' is unknown.");
                }

                if (errors.Count != 0)
                {
                    foreach (var err in errors)
                        Console.Error.WriteLine(err, ERROR_COLOR);
                    app.ShowHelp();
                    return 1;
                }

                var schemaLoader = schemaType.GetSchemaLoader();
                await CodeGenRunner.RunAsync(sourceFile.Value, schemaLoader, templatesPaths.Values, outputPath.Value, duplicatesTemplateHandlingStrategy);

                return 0;
            });

            try
            {
                var result = app.Execute(args);
#if (!DEBUG)
                 Environment.Exit(result);
#endif

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
#if (!DEBUG)
                Environment.Exit(1);
#endif
            }
        }
    }
}
