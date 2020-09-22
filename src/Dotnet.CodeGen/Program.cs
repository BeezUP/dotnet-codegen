using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Console = Colorful.Console;

namespace Dotnet.CodeGen.CodeGen
{
    public class Program
    {
        public static readonly Color ERROR_COLOR = Color.OrangeRed;

        public const string SOURCE_FILE_OPTION = "-s|--source";
        public const string OUTPUT_PATH_OPTION = "-o|--output";

        public const string TEMPLATES_PATH_OPTION = "-t|--template";
        public const string TEMPLATE_DUPLICATES_HANDLING_STRATEGY_OPTION = "-d|--duplicates";
        public const string SCHEMA_LOADER_TYPE_OPTION = "-l|--loader";

        public const string AUTHORIZATION_OPTION = "-a|--auth";

        public const string INTERMEDIATE_OPTION = "-i|--intermediate";

        private const string HelpOptions = "-?|-h|--help";

        public static async Task Main(string[] args)
        {
            //https://msdn.microsoft.com/fr-fr/magazine/mt763239.aspx
            var app = new CommandLineApplication();
            app.HelpOption(HelpOptions);

            var schemaTypeName = app.Option(SCHEMA_LOADER_TYPE_OPTION, $"Enter a schema loader type between those values [{string.Join(" | ", Enum.GetNames(typeof(SourceSchemaType)))}]", CommandOptionType.SingleValue);
            var duplicatesTemplateHandlingStrategyName = app.Option(TEMPLATE_DUPLICATES_HANDLING_STRATEGY_OPTION, $"Enter a template duplication handling strategy between those values [{string.Join(" | ", Enum.GetNames(typeof(TemplateDuplicationHandlingStrategy)))}]", CommandOptionType.SingleValue);

            var sourceFiles = app.Option(SOURCE_FILE_OPTION, "Enter a path (relative or absolute) to an source document.", CommandOptionType.MultipleValue);
            var authorization = app.Option(AUTHORIZATION_OPTION, "Enter an authorization token to access source documents", CommandOptionType.SingleValue);
            var outputPath = app.Option(OUTPUT_PATH_OPTION, "Enter the path (relative or absolute) to the output path (content will be overritten)", CommandOptionType.SingleValue);
            var templatesPaths = app.Option(TEMPLATES_PATH_OPTION, "Enter a path (relative or absolute / file or folder) to a template.", CommandOptionType.MultipleValue);
            var intermediate = app.Option(INTERMEDIATE_OPTION, "Enter a path (relative or absolute) to a file for intermediate 'all refs merged' output of the json document", CommandOptionType.SingleValue);

            app.OnExecute(async () =>
            {
                var errors = new List<string>();
                if (sourceFiles.Values == null || sourceFiles.Values.Count == 0)
                    errors.Add($"At least one source file ({SOURCE_FILE_OPTION}) option is required.");
                if (templatesPaths.Values == null || templatesPaths.Values.Count == 0)
                    errors.Add($"At least one tempalte ({TEMPLATES_PATH_OPTION}) option is required.");
                if (outputPath.Values == null || outputPath.Values.Count != 1)
                    errors.Add($"Exactly one output path ({OUTPUT_PATH_OPTION}) option is required.");

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

                var auth = authorization.HasValue() ? authorization.Value() : null;

                try
                {
                    var schemaLoader = schemaType.GetSchemaLoader();
                    var jsonObject = await schemaLoader.LoadSchemaAsync(sourceFiles.Values, auth);

                    if (intermediate.HasValue())
                    {
                        File.WriteAllText(intermediate.Value(), jsonObject.ToString());
                    }

                    await CodeGenRunner.RunAsync(jsonObject, templatesPaths.Values, outputPath.Value(), duplicatesTemplateHandlingStrategy);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.GetBaseException().Message);
                    throw;
                }

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
