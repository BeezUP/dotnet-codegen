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
        static public IEnumerable<IBlockHelper> BlockHelpers { get { foreach (var h in _blockHelpers) yield return h; } }
        static public IEnumerable<IStandardHelper> StandardHelpers { get { foreach (var h in _standardHelpers) yield return h; } }


        static readonly IBlockHelper[] _blockHelpers;
        static readonly IStandardHelper[] _standardHelpers;


        static HandlebarsConfigurationHelper()
        {
            var blockHelpers = new List<IBlockHelper>();
            var standardHelpers = new List<IStandardHelper>();

            foreach (var type in typeof(HandlebarsConfigurationHelper).Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                )
            {
                if (typeof(IBlockHelper).IsAssignableFrom(type))
                {
                    var ctor = type.GetConstructor(new Type[0]);
                    var instance = ctor.Invoke(new object[0]);
                    blockHelpers.Add((IBlockHelper)instance);
                }
                if (typeof(IStandardHelper).IsAssignableFrom(type))
                {
                    var ctor = type.GetConstructor(new Type[0]);
                    var instance = ctor.Invoke(new object[0]);
                    standardHelpers.Add((IStandardHelper)instance);
                }
            }

            _blockHelpers = blockHelpers.ToArray();
            _standardHelpers = standardHelpers.ToArray();
        }


        public static IHandlebars GetHandlebars(string rootDirectory)
        {
            var handlebars = Handlebars.Create(GetHandlebarsConfiguration(rootDirectory));
            return handlebars;
        }

        public static HandlebarsConfiguration GetHandlebarsConfiguration(string rootDirectory)
        {
            var configuration = new HandlebarsConfiguration();
            foreach (var h in _blockHelpers)
            {
                configuration.BlockHelpers.Add(h.Name, h.Helper);
            }
            foreach (var h in _standardHelpers)
            {
                configuration.Helpers.Add(h.Name, h.Helper);
            }

            configuration.PartialTemplateResolver = new SameDirectoryPartialTemplateResolver(rootDirectory);

            return configuration;
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
                var partialPath = Path.Combine(_rootDirectory, $"_{partialName}.hbs");
                if (!File.Exists(partialPath))
                {
                    return false;
                    throw new IOException($"Unable to find the partial template file {partialPath}");
                }

                env.RegisterTemplate(partialName, File.ReadAllText(partialPath));
                return true;
            }
        }
    }
}
