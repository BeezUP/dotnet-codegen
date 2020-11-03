using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodegenUP.CustomHandlebars
{
    public static class HandlebarsConfigurationHelper
    {
        static public IEnumerable<IHelper> DefaultHelpers { get { foreach (var h in _Helpers) yield return h; } }

        static readonly IHelper[] _Helpers;

#pragma warning disable CA1810 // Initialize reference type static fields inline
#pragma warning disable S3963 // "static" fields should be initialized inline
        static HandlebarsConfigurationHelper()
        {
            var thisAssembly = typeof(HandlebarsConfigurationHelper).Assembly;
            _Helpers = GetHelpersFromAssembly(thisAssembly).ToArray();
        }

        public static IHelper[] GetHelpersFromFolder(string projectFolderPath, string artifactDirectory)
        {
            var csproj = Directory.GetFiles(projectFolderPath, "*.csproj").FirstOrDefault() ?? throw new InvalidDataException($"No .csproj file found in the directory {projectFolderPath}");
            var projName = Path.GetFileNameWithoutExtension(csproj);
            string outputFolder = Path.Combine(artifactDirectory, projName);

            using (var consoleOut = Console.OpenStandardOutput()) ;

            // Build the project
            var dotnetCommand = $"build {csproj} -o \"{outputFolder}\" -c Release";
            var start = new ProcessStartInfo("dotnet", dotnetCommand)
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            var process = new Process
            {
                StartInfo = start,
            };
            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Console.WriteLine(output);
                throw new InvalidOperationException($"Something bad happened when building the project '{csproj}'");
            }

            // Dynamic load of the assemblies, starting with the one with some helper
            var helpers = new List<IHelper>();
            var assemblyName = $"{projName}.dll";
            var assemblyFilePath = Directory.GetFiles(outputFolder, assemblyName).FirstOrDefault() ?? throw new InvalidDataException($"No .csproj file found in the directory {projectFolderPath}"); ;
            var loadedAssembly = Assembly.LoadFrom(assemblyFilePath);
            var helps = GetHelpersFromAssembly(loadedAssembly);
            helpers.AddRange(helps);

            return helpers.ToArray();
        }

        public static List<IHelper> GetHelpersFromAssembly(Assembly thisAssembly)
        {
            var helpers = new List<IHelper>();
            foreach (var type in GetHelperTypesFromAssembly(thisAssembly))
            {
                var ctor = type.GetConstructor(new Type[0]) ?? throw new InvalidProgramException("Helpers implementations should have a public, parameterless ctor");
                var instance = ctor.Invoke(new object[0]);
                helpers.Add((IHelper)instance);
            }
            return helpers;
        }

        public static IEnumerable<Type> GetHelperTypesFromAssembly(Assembly thisAssembly)
        {
            var iHelperType = typeof(IHelper);
            foreach (var type in thisAssembly.GetTypes().Where(t => !t.IsAbstract && !t.IsInterface))
            {
                if (iHelperType.IsAssignableFrom(type))
                {
                    yield return type;
                }
            }
        }

#pragma warning restore S3963 // "static" fields should be initialized inline
#pragma warning restore CA1810 // Initialize reference type static fields inline

        private static void RegisterDefaultHelpers(this HandlebarsConfiguration handlebarsConfiguration)
            => RegisterHelpers(handlebarsConfiguration, _Helpers);

        private static void RegisterHelpers(this HandlebarsConfiguration handlebarsConfiguration, IEnumerable<IHelperBase> helpers)
        {
            foreach (var h in helpers)
                h.Setup(handlebarsConfiguration);
        }

        public static IHandlebars GetHandlebars(string rootDirectory, IEnumerable<IHelperBase> moreHelpers) => Handlebars.Create(GetHandlebarsConfiguration(rootDirectory, moreHelpers));
        public static IHandlebars GetHandlebars(IEnumerable<string> directories, IEnumerable<IHelperBase> moreHelpers) => Handlebars.Create(GetHandlebarsConfiguration(directories, moreHelpers));

        public static HandlebarsConfiguration GetHandlebarsConfiguration(string rootDirectory, IEnumerable<IHelperBase> moreHelpers)
            => GetHandleBarsConfiguration(new SameDirectoryPartialTemplateResolver(rootDirectory), moreHelpers);
        public static HandlebarsConfiguration GetHandlebarsConfiguration(IEnumerable<string> directories, IEnumerable<IHelperBase> moreHelpers)
            => GetHandleBarsConfiguration(new MultipleDirectoriesPartialTemplateResolver(directories), moreHelpers);

        private static HandlebarsConfiguration GetHandleBarsConfiguration(IPartialTemplateResolver templateResolver, IEnumerable<IHelperBase> moreHelpers)
        {
            var configuration = new HandlebarsConfiguration();
            configuration.RegisterDefaultHelpers();
            configuration.RegisterHelpers(moreHelpers);
            configuration.PartialTemplateResolver = templateResolver;
            return configuration;
        }

        private static bool TryRegisterPartialFile(string directory, IHandlebars env, string partialName)
        {
            var partialPath = Path.Combine(directory, $"_{partialName}.hbs");
            if (!File.Exists(partialPath))
            {
                return false;
            }
            env.RegisterTemplate(partialName, File.ReadAllText(partialPath));
            return true;
        }

        class MultipleDirectoriesPartialTemplateResolver : IPartialTemplateResolver
        {
            private readonly string[] _directories;

            public MultipleDirectoriesPartialTemplateResolver(IEnumerable<string> directories)
            {
                _directories = directories.ToArray();
            }

            public bool TryRegisterPartial(IHandlebars env, string partialName, string templatePath)
            {
                foreach (var directory in _directories)
                {
                    if (TryRegisterPartialFile(directory, env, partialName))
                        return true;
                }
                return false;
            }
        }

        class SameDirectoryPartialTemplateResolver : IPartialTemplateResolver
        {
            private readonly string _rootDirectory;

            public SameDirectoryPartialTemplateResolver(string rootDirectory)
            {
                _rootDirectory = rootDirectory;
            }

            public bool TryRegisterPartial(IHandlebars env, string partialName, string templatePath)
            {
                return TryRegisterPartialFile(_rootDirectory, env, partialName);
            }
        }
    }
}
