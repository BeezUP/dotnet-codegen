using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Dotnet.CodeGen.CustomHandlebars
{
    public static class HandlebarsConfigurationHelper
    {
        static public IEnumerable<IHelper> Helpers { get { foreach (var h in _Helpers) yield return h; } }

        static readonly IHelper[] _Helpers;

#pragma warning disable CA1810 // Initialize reference type static fields inline
#pragma warning disable S3963 // "static" fields should be initialized inline
        static HandlebarsConfigurationHelper()
        {
            var helpers = new List<IHelper>();

            foreach (var type in typeof(HandlebarsConfigurationHelper).Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                )
            {
                if (typeof(IHelper).IsAssignableFrom(type))
                {
                    var ctor = type.GetConstructor(new Type[0]);
                    var instance = ctor.Invoke(new object[0]);
                    helpers.Add((IHelper)instance);
                }
            }

            _Helpers = helpers.ToArray();
        }
#pragma warning restore S3963 // "static" fields should be initialized inline
#pragma warning restore CA1810 // Initialize reference type static fields inline

        private static void RegisterHelpers(HandlebarsConfiguration handlebarsConfiguration)
        {
            foreach (var h in _Helpers)
            {
                h.Setup(handlebarsConfiguration);
            }
        }

        public static IHandlebars GetHandlebars(string rootDirectory) => Handlebars.Create(GetHandlebarsConfiguration(rootDirectory));
        public static IHandlebars GetHandlebars(IEnumerable<string> directories) => Handlebars.Create(GetHandlebarsConfiguration(directories));

        public static HandlebarsConfiguration GetHandlebarsConfiguration(string rootDirectory)
            => GetHandleBarsConfiguration(new SameDirectoryPartialTemplateResolver(rootDirectory));
        public static HandlebarsConfiguration GetHandlebarsConfiguration(IEnumerable<string> directories)
            => GetHandleBarsConfiguration(new MultipleDirectoriesPartialTemplateResolver(directories));

        private static HandlebarsConfiguration GetHandleBarsConfiguration(IPartialTemplateResolver templateResolver)
        {
            var configuration = new HandlebarsConfiguration();
            RegisterHelpers(configuration);
            configuration.PartialTemplateResolver = templateResolver;
            return configuration;
        }

        private static bool TryRegisterPartialFile(string directory, IHandlebars env, string partialName)
        {
            var partialPath = Path.Combine(directory, $"_{partialName}.hbs");
            if (!File.Exists(partialPath))
            {
                //return false;
                throw new IOException($"Unable to find the partial template file {partialPath}");
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
                var result = true;
                foreach (var directory in _directories)
                {
                    result = result && TryRegisterPartialFile(directory, env, partialName);
                }
                return result;
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
