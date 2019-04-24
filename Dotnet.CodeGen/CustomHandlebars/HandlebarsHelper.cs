using HandlebarsDotNet;

namespace Dotnet.CodeGen.CodeGen.CustomHandlebars
{
    public static class HandlebarsHelper
    {
        public static IHandlebars GetHandlebars()
        {
            var handlebars = Handlebars.Create(GetHandlebarsConfiguration());
            return handlebars;
        }

        public static HandlebarsConfiguration GetHandlebarsConfiguration()
        {
            var response = new HandlebarsConfiguration
            {
                BlockHelpers =
                {
                    {"each_with_sort", CodeGenHandlebarsHelpers.EachWithSort},
                    {"is_required", CodeGenHandlebarsHelpers.IsRequiredHelper},
                    {"is_last_object_property", CodeGenHandlebarsHelpers.IsLastObjectProperty},
                    {"is_enum", CodeGenHandlebarsHelpers.IsEnum},
                    {"is_enum_default", CodeGenHandlebarsHelpers.IsEnumDefault},
                    {"are_equal", CodeGenHandlebarsHelpers.AreEqual }
                },
                Helpers =
                {
                    {"uppercase_first_letter", CodeGenHandlebarsHelpers.UppercaseFirstLetterHelper}
                }
            };

            return response;
        }
    }
}
