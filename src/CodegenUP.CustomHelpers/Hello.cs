using CodegenUP.CustomHandlebars;
using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Hello world helper
/// </summary>
[HandlebarsHelperSpecification("{ prop: 'world'}", "{{hello prop }}", "hello world!")]
public class Hello : SimpleStandardHelperBase
{
    public Hello() : base("hello") { }

    public override HandlebarsHelper Helper =>
        (TextWriter output, object context, object[] arguments) =>
        {
            EnsureArgumentsCount(arguments, 1);

            var argument = arguments[0].ToString();

            output.Write($"hello {argument}!");
        };
}
