using CodegenUP.CustomHandlebars;
using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodegenUP
{
    public class GlobalParametersHelper : IHelperBase
    {
        public const string NAME = "global_parameter";

        private readonly IReadOnlyDictionary<string, string> _values;

        public GlobalParametersHelper(IReadOnlyDictionary<string, string> values)
        {
            _values = values;
        }

        public void Setup(HandlebarsConfiguration configuration)
        {
            configuration.Helpers.Add(NAME, HelperFunction);
        }

        void HelperFunction(TextWriter output, object context, object[] arguments)
        {
            string? key = arguments.Length > 0 ? arguments[0]?.ToString() : null;
            if (key == null)
                throw new CodeGenHelperException(NAME, "Helper needs the key as first parameter");

            if (_values.TryGetValue(key, out var value))
            {
                output.Write(value);
            }
            else
            {
                output.Write($"No {NAME} found for key '{key}'");
            }
        }
    }
}
