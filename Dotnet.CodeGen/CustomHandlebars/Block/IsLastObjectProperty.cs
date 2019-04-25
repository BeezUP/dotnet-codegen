using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars.Block
{
    //[HandlebarsHelperSpecification("{ test: 'AA' }", "{{uppercase_first_letter test}}", "AA")]
    public class IsLastObjectProperty : BlockHelperBase
    {
        public IsLastObjectProperty() :
            base(
                "is_last_object_property",
                () => (TextWriter output, HelperOptions options, dynamic context, object[] arguments) =>
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
                })
        {
        }
    }
}
