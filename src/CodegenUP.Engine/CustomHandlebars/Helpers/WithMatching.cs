using HandlebarsDotNet;
using System;
using System.IO;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// Execute the inner template with the matching parameter, when matching key is equal to the first parameter
    /// {{#with_matching some_value matching_key1 context1 mateching_key2 context2 ... }}
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{#with_matching 'test' '1' '1', '2', '2'}}{{else}}NOT FOUND{{/with_matching}}", "NOT FOUND")]
    [HandlebarsHelperSpecification("{}", "{{#with_matching 'value1' 'value1' 'context1', '2', '2'}}{{.}}{{else}}NOT FOUND{{/with_matching}}", "context1")]
    [HandlebarsHelperSpecification("{ value: '42' }", "{{#with_matching value '42' . }}{{value}}{{else}}NOT FOUND{{/with_matching}}", "42")]
#endif
    public class WithMatching : SimpleBlockHelperBase
    {
        public WithMatching() : base("with_matching") { }

        public override void Helper(TextWriter output, HelperOptions options, object context, object[] arguments)
        {
            if (arguments.Length % 2 != 1)
                throw new CodeGenHelperException($"Arguments number for the {Name} helper must be an odd number");

            var value = GetArgumentAs<string>(arguments, 0) ?? "";

            var pair_position = 1;
            while (pair_position < arguments.Length)
            {
                var match_key = GetArgumentAs<string>(arguments, pair_position) ?? "";
                if (string.Compare(value, match_key, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    options.Template(output, arguments[pair_position + 1]);
                    return;
                }

                pair_position += 2;
            }

            options.Inverse(output, context);
        }
    }
}
