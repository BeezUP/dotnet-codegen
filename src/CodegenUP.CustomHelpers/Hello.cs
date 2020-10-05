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

    public override void Helper(TextWriter output, object context, object[] arguments)
    {
        EnsureArgumentsCount(arguments, 1);

        var argument = arguments[0].ToString();

        output.Write($"hello {argument}!");
    }
}

/// <summary>
/// Hello world helper
/// </summary>
[HandlebarsHelperSpecification("{ prop: 'world'}", "{{hello2 prop }}", "hello world!")]
public class SameHello : SimpleStandardHelperBase<object, string>
{
    public SameHello() : base("hello2") { }

    public override void HelperFunction(TextWriter output, object context, string name, object[] arguments)
    {
        EnsureArgumentsCount(arguments, 0);
        output.Write($"hello {name}!");
    }
}
