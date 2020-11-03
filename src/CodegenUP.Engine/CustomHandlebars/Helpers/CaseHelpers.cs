using HandlebarsDotNet;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// Pascal case the string
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{ test: 42 }", "{{pascal_case test}}", "42")]
    [HandlebarsHelperSpecification("{ test: 'HELLO' }", "{{pascal_case test}}", "HELLO")]
    [HandlebarsHelperSpecification("{ test: 'HelloWorld' }", "{{pascal_case test}}", "HelloWorld")]
    [HandlebarsHelperSpecification("{ test: 'hello' }", "{{pascal_case test}}", "Hello")]
    [HandlebarsHelperSpecification("{ test: 'heLlo wOrld' }", "{{pascal_case test}}", "HeLloWOrld")]
    [HandlebarsHelperSpecification("{ test: 'hello_world' }", "{{pascal_case test}}", "HelloWorld")]
    [HandlebarsHelperSpecification("{ test: 'hello-world' }", "{{pascal_case test}}", "HelloWorld")]
    [HandlebarsHelperSpecification("{ test: 'hello-WORLD' }", "{{pascal_case test}}", "HelloWORLD")]
#endif
    public class PascalCase : SimpleStandardHelperBase<object, string>
    {
        public PascalCase() : base("pascal_case") { }
        protected PascalCase(string name) : base(name) { }

        public override void HelperFunction(TextWriter output, object context, string toCase, object[] otherArguments)
        {
            output.Write(StringHelpers.ToPascalCase(toCase));
        }
    }


    /// <summary>
    /// Camel case the string
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{ test: 42 }", "{{camel_case test}}", "42")]
    [HandlebarsHelperSpecification("{ test: 'HELLO' }", "{{camel_case test}}", "hELLO")]
    [HandlebarsHelperSpecification("{ test: 'hello' }", "{{camel_case test}}", "hello")]
    [HandlebarsHelperSpecification("{ test: 'heLlo wOrld' }", "{{camel_case test}}", "heLloWOrld")]
    [HandlebarsHelperSpecification("{ test: 'hello_world' }", "{{camel_case test}}", "helloWorld")]
    [HandlebarsHelperSpecification("{ test: 'hello-world' }", "{{camel_case test}}", "helloWorld")]
    [HandlebarsHelperSpecification("{ test: 'hello-WORLD' }", "{{camel_case test}}", "helloWORLD")]
    [HandlebarsHelperSpecification("{ test: 'HelloWorld' }", "{{camel_case test}}", "helloWorld")]
#endif
    public class CamelCase : SimpleStandardHelperBase<object, string>
    {
        public CamelCase() : base("camel_case") { }
        protected CamelCase(string name) : base(name) { }

        public override void HelperFunction(TextWriter output, object context, string toCase, object[] otherArguments)
        {
            output.Write(StringHelpers.ToCamelCase(toCase));
        }
    }

    /// <summary>
    /// Snake case the string
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{ test: 42 }", "{{snake_case test}}", "42")]
    [HandlebarsHelperSpecification("{ test: 'hello ' }", "{{snake_case test}}", "hello")]
    [HandlebarsHelperSpecification("{ test: 'hello world' }", "{{snake_case test}}", "hello_world")]
    [HandlebarsHelperSpecification("{ test: 'hello_world' }", "{{snake_case test}}", "hello_world")]
    [HandlebarsHelperSpecification("{ test: 'hello-world' }", "{{snake_case test}}", "hello_world")]
    [HandlebarsHelperSpecification("{ test: 'hello--world' }", "{{snake_case test}}", "hello_world")]
    [HandlebarsHelperSpecification("{ test: 'hello__World' }", "{{snake_case test}}", "hello_world")]
    [HandlebarsHelperSpecification("{ test: 'hello-World' }", "{{snake_case test}}", "hello_world")]
    [HandlebarsHelperSpecification("{ test: 'hello _ world' }", "{{snake_case test}}", "hello_world")]
    [HandlebarsHelperSpecification("{ test: 'hello - world' }", "{{snake_case test}}", "hello_world")]
    [HandlebarsHelperSpecification("{ test: 'HelloWorld' }", "{{snake_case test}}", "hello_world")]
    [HandlebarsHelperSpecification("{ test: 'hello _WORLD' }", "{{snake_case test}}", "hello_world")]
    [HandlebarsHelperSpecification("{ test: ' HELLO' }", "{{snake_case test}}", "hello")]
    [HandlebarsHelperSpecification("{ test: 'Hello ' }", "{{snake_case test}}", "hello")]
    [HandlebarsHelperSpecification("{ test: '2Hello2 ' }", "{{snake_case test}}", "2_hello2")]
    [HandlebarsHelperSpecification("{ test: 'HelloWorld_42LongName ' }", "{{snake_case test}}", "hello_world_42_long_name")]
#endif
    public class SnakeCase : SimpleStandardHelperBase<object, string>
    {
        public SnakeCase() : base("snake_case") { }
        protected SnakeCase(string name) : base(name) { }

        public override void HelperFunction(TextWriter output, object context, string toCase, object[] otherArguments)
        {
            output.Write(StringHelpers.ToSnakeCase(toCase));
        }
    }
}
