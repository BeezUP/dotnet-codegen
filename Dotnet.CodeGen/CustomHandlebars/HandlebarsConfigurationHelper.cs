using HandlebarsDotNet;
using System;
using System.Collections.Generic;
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


        public static IHandlebars GetHandlebars()
        {
            var handlebars = Handlebars.Create(GetHandlebarsConfiguration());
            return handlebars;
        }

        public static HandlebarsConfiguration GetHandlebarsConfiguration()
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

            //{
            //    BlockHelpers =
            //    {
            //        { "each_with_sort", HandlebarsBlockHelpers.EachWithSort},
            //        { "is_required", HandlebarsBlockHelpers.IsRequiredHelper},
            //        { "is_last_object_property", HandlebarsBlockHelpers.IsLastObjectProperty},
            //        { "is_enum", HandlebarsBlockHelpers.IsEnum},
            //        { "is_enum_default", HandlebarsBlockHelpers.IsEnumDefault},
            //        { "are_equal", HandlebarsBlockHelpers.AreEqual }
            //    },
            //    Helpers =
            //    {
            //        { "uppercase_first_letter", HandlebarsStandardHelpers.UppercaseFirstLetterHelper}
            //    }
            //};

            return configuration;
        }
    }
}
