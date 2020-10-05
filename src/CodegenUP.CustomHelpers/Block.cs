using CodegenUP.CustomHandlebars;
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

    public override void Helper(TextWriter output, HelperOptions options, object context, object[] arguments)
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
    }
}

[HandlebarsHelperSpecification("{ prop: 'ok'}", "{{#is_ok2 prop}}OK{{else}}NOK{{/is_ok2}}", "OK")]
[HandlebarsHelperSpecification("{ prop: 'plop'}", "{{#is_ok2 prop}}OK{{else}}NOK{{/is_ok2}}", "NOK")]
public class SameBlock : SimpleBlockHelperBase<object, string>
{
    public SameBlock() : base("is_ok2") { }

    public override void HelperFunction(TextWriter output, HelperOptions options, object context, string argument, object[] otherArguments)
    {
        EnsureArgumentsCountMax(otherArguments, 0);

        var condition = argument == "ok";
        if (condition)
        {
            options.Template(output, context);
        }
        else
        {
            options.Inverse(output, context);
        }
    }
}
