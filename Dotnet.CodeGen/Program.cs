using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Console = Colorful.Console;

namespace Dotnet.CodeGen.CodeGen
{
    class Program
    {
        public static readonly Color ERROR_COLOR = Color.OrangeRed;

        public const string SOURCE_FILE_ARGUMENT = "source";
        public const string TEMPLATE_PATH_OPTION = "-t | --template";
        public const string OUTPUT_PATH_OPTION = "-o | --out";
        public const string SCHEMA_TYPE_OPTION = "--type";
        public const SourceSchemaType DEFAULT_SCHEMA_TYPE = SourceSchemaType.OpenApi;
        private const string HelpOptions = "-? | -h | --help";

        static async Task Main(string[] args)
        {
            //https://msdn.microsoft.com/fr-fr/magazine/mt763239.aspx
            var app = new CommandLineApplication();
            app.HelpOption(HelpOptions);

            var sourceFile = app.Argument(SOURCE_FILE_ARGUMENT, "Enter the path (relative or absolute) to an source document.");
            var templatePath = app.Option(TEMPLATE_PATH_OPTION, "Enter the path (relative or absolute / single file or folder) to the template.", CommandOptionType.SingleValue);
            var outputPath = app.Option(OUTPUT_PATH_OPTION, "Enter the path (relative or absolute) to the output path (content will be overritten)", CommandOptionType.SingleValue);
            var schemaTypeName = app.Option(SCHEMA_TYPE_OPTION, $"Enter a schema type between those values [{string.Join(" | ", Enum.GetNames(typeof(SourceSchemaType)))}]", CommandOptionType.SingleValue);


            app.OnExecute(async () =>
            {
                var errors = new List<string>();
                if (sourceFile.Value == null)
                    errors.Add($"{SOURCE_FILE_ARGUMENT} parameter is required");
                if (!templatePath.HasValue())
                    errors.Add($"{TEMPLATE_PATH_OPTION} parameter is required");
                if (!outputPath.HasValue())
                    errors.Add($"{OUTPUT_PATH_OPTION} parameter is required");

                var schemaType = DEFAULT_SCHEMA_TYPE;
                if (schemaTypeName.HasValue())
                {
                    if (Enum.TryParse<SourceSchemaType>(schemaTypeName.Value(), out var s))
                        schemaType = s;
                    else
                        errors.Add($"The schema type '{schemaTypeName.Value()}' is unknown.");
                }

                if (errors.Count != 0)
                {
                    foreach (var err in errors)
                        Console.Error.WriteLine(err, ERROR_COLOR);
                    app.ShowHelp();
                    return 1;
                }

                // dotnet run -- ./_samples/swagger.json -o ././

                var schemaLoader = schemaType.GetSchemaLoader();

                await CodeGenRunner.RunAsync(sourceFile.Value, schemaLoader, templatePath.Value(), outputPath.Value());

                return 0;
            });

            try
            {
                app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }
    }
}
