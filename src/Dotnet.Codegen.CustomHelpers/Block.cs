using Dotnet.CodeGen.CustomHandlebars;
using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Hello world helper
/// </summary>
[HandlebarsHelperSpecification("{ prop: 'ok'}", "{{#is_ok prop}}OK{{else}}NOK{{/is_ok}}", "OK")]
[HandlebarsHelperSpecification("{ prop: 'plop'}", "{{#is_ok prop}}OK{{else}}NOK{{/is_ok}}", "NOK")]
public class Block : SimpleBlockHelperBase
{
    public Block() : base("is_ok") { }

    public override HandlebarsBlockHelper Helper =>
            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
            {
                EnsureArgumentsCountMax(arguments, 1);

                var condition = arguments[0]?.ToString() == "ok";
                if (condition)
                {
                    options.Template(output, context);
                }
                else
                {
                    options.Inverse(output, context);
                }
            };
}
