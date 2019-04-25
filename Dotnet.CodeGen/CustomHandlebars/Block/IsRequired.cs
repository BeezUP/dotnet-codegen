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
    public class IsRequired : BlockHelperBase
    {
        public IsRequired() :
            base(
                "is_required",
                () => (TextWriter output, HelperOptions options, dynamic context, object[] arguments) =>
                {
                    var argument = arguments.FirstOrDefault() as string;
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
                })
        {
        }
    }
}
