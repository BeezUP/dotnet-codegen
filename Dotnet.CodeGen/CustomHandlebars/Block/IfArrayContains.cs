using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars.Block
{
    [HandlebarsHelperSpecification(@"
{
    'type' : 'object',
    'required' : [ 'errorMessage' ],
    'properties' : {
        'errorMessage' : {
            'type' : 'string'
        },
        'non_required_prop' : {
            'type' : 'string'
        }
    }
}", "{{#each properties}} {{is_required @key}}OK {{else}} {{/is_required}}  {{/each}}", "OK")]
    public class IfArrayContains : BlockHelperBase
    {
        public IfArrayContains() : base("is_required") { }

        public override HandlebarsBlockHelper Helper =>
            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
                {
                    EnsureArgumentsCount(arguments, 2);

                    var argument = arguments.FirstOrDefault() as string;

                    if (!string.IsNullOrEmpty(argument))
                    {
                        // First we look at the ancestors of the property node for the list of "required" properties
                        var requiredProperties = GetJContainerContext(context)
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
                };
    }
}
