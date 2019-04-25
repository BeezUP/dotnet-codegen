using System.IO;
using System.Linq;
using HandlebarsDotNet;
using Newtonsoft.Json.Linq;

namespace Dotnet.CodeGen.CustomHandlebars
{
    /// <summary>
    /// Container for handlerbars block helpers
    /// </summary>
    public static class HandlebarsBlockHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <param name="arguments"></param>
        public static void EachWithSort(TextWriter output, HelperOptions options, dynamic context, params object[] arguments)
        {
            var children = ((JObject)arguments.First()).Children().Select(x => (JProperty)x).ToArray();

            var ret = new JObject(children.OrderBy(x => x.Name));

            options.Template(output, ret);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <param name="arguments"></param>
        public static void IsRequiredHelper(TextWriter output, HelperOptions options, dynamic context, object[] arguments)
        {
            var argument = arguments.First() as string;
            if (!string.IsNullOrEmpty(argument))
            {
                // First we look at the ancestors of the property node for the list of "required" properties
                var requiredProperties = ((JContainer)context)
                    .Ancestors()
                    .OfType<JObject>()
                    .FirstOrDefault(jObject => jObject.ContainsKey("required"))
                    ?.Property("required")
                    ?.Value;

                if (requiredProperties == null)
                {
                    options.Inverse(output, null);
                }
                else
                {
                    var isRequiredProperty = requiredProperties.Any(req => req.Value<string>() == argument);
                    if (isRequiredProperty)
                    {
                        options.Template(output, null);
                    }
                    else
                    {
                        options.Inverse(output, null);
                    }
                }
            }
        }

        public static void IsLastObjectProperty(TextWriter output, HelperOptions options, dynamic context, object[] arguments)
        {
            var argument = arguments.First() as string;
            if (!string.IsNullOrEmpty(argument))
            {
                var siblingProperties = ((JContainer)context)
                    .Ancestors()
                    .OfType<JObject>()
                    .FirstOrDefault(jObject => jObject.ContainsKey("properties"))
                    ?.Property("properties")
                    ?.Value;

                var lastPropertyName = siblingProperties?.Values<JProperty>()?.LastOrDefault()?.Name;
                if (lastPropertyName == argument)
                {
                    options.Template(output, null);
                }
                else
                {
                    options.Inverse(output, null);
                }
            }
        }

        public static void IsEnum(TextWriter output, HelperOptions options, dynamic context, object[] arguments)
        {
            var enumProperty = ((JObject)context)?.Property("enum");

            if (enumProperty != null)
            {
                options.Template(output, context);
            }
            else
            {
                options.Inverse(output, context);
            }
        }

        public static void IsEnumDefault(TextWriter output, HelperOptions options, dynamic context, object[] arguments)
        {
            var argument = arguments.First() as string;

            var definition = (JObject)context.Parent.Parent.Parent;
            var enumProperty = definition.Property("enum");
            var defaultProperty = definition.Property("default");

            if (enumProperty != null
                && defaultProperty?.Value.ToString() == argument)
            {
                options.Template(output, context);
            }
            else
            {
                options.Inverse(output, context);
            }
        }

        public static void AreEqual(TextWriter output, HelperOptions options, dynamic context, object[] arguments)
        {
            if (arguments.Length != 2)
            {
                options.Inverse(output, context);
            }
            else
            {
                string arg;
                if (arguments[0] is JValue t)
                {
                    arg = t.Value.ToString();
                }
                else
                {
                    arg = arguments[0] as string;
                }

                var expected = arguments[1] as string;

                if (arg == expected)
                {
                    options.Template(output, context);
                }
                else
                {
                    options.Inverse(output, context);
                }
            }
        }
    }
}
