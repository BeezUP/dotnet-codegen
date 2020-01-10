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


        public static IHandlebars GetHandlebars(string rootDirectory)
        {
            var handlebars = Handlebars.Create(GetHandlebarsConfiguration(rootDirectory));
            return handlebars;
        }

        public static IHandlebars GetHandlebars(List<string> directories)
        {
            var handlebars = Handlebars.Create(GetHandlebarsConfiguration(directories));
            return handlebars;
        }

        private static void RegisterHelpers(HandlebarsConfiguration handlebarsConfiguration)
        {
            foreach (var h in _Helpers)
            {
                h.Setup(handlebarsConfiguration);
            }
        }
        
        public static HandlebarsConfiguration GetHandlebarsConfiguration(string rootDirectory)
        {
            var configuration = new HandlebarsConfiguration();

            RegisterHelpers(configuration);

            configuration.PartialTemplateResolver = new SameDirectoryPartialTemplateResolver(rootDirectory);

            return configuration;
        }

        public static HandlebarsConfiguration GetHandlebarsConfiguration(List<string> directories)
        {
            var configuration = new HandlebarsConfiguration();

            RegisterHelpers(configuration);

            configuration.PartialTemplateResolver = new MultipleDirectoriesPartialTemplateResolver(directories);

            return configuration;
        }

        private static bool TryRegisterPartial(string directory, IHandlebars env, string partialName)
        {
            var partialPath = Path.Combine(directory, $"_{partialName}.hbs");
            if (!File.Exists(partialPath))
            {
                return false;
                throw new IOException($"Unable to find the partial template file {partialPath}");
            }

            env.RegisterTemplate(partialName, File.ReadAllText(partialPath));
            return true;
        }

        public class MultipleDirectoriesPartialTemplateResolver : IPartialTemplateResolver
        {
            private readonly List<string> _directories;

            public MultipleDirectoriesPartialTemplateResolver(List<string> directories)
            {
                _directories = directories;
            }

            public bool TryRegisterPartial(IHandlebars env, string partialName, string templatePath)
            {
                var result = true;

                foreach (var directory in _directories)
                {
                    result = result && HandlebarsConfigurationHelper.TryRegisterPartial(directory, env, partialName);
                }

                return result;
            }
        }

        public class SameDirectoryPartialTemplateResolver : IPartialTemplateResolver
        {
            private readonly string _rootDirectory;

            public SameDirectoryPartialTemplateResolver(string rootDirectory)
            {
                _rootDirectory = rootDirectory;
            }

            public bool TryRegisterPartial(IHandlebars env, string partialName, string templatePath)
            {
                return HandlebarsConfigurationHelper.TryRegisterPartial(_rootDirectory, env, partialName);
            }
        }
    }
}
