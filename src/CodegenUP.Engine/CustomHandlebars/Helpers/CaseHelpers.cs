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
            if (!string.IsNullOrEmpty(toCase))
            {
                var chars = toCase.Trim().ToArray();
                var seed = (upperNext: true, result: new StringBuilder());
                var aggregated = chars.Aggregate(seed, (s, c) =>
                {
                    var (upperNext, sb) = s;

                    if (c == ' ' || c == '_' || c == '-')
                    {
                        return (upperNext: true, sb);
                    }

                    sb.Append(upperNext ?
                        c.ToString().ToUpperInvariant() :
                        c.ToString()
                        );

                    return (false, sb);
                });

                output.Write(aggregated.result.ToString());
            }
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
            if (!string.IsNullOrEmpty(toCase))
            {
                var chars = toCase.Trim().ToArray();
                var seed = (upperNext: (bool?)false, result: new StringBuilder());
                var aggregated = chars.Aggregate(seed, (s, c) =>
                {
                    var (upperNext, sb) = s;

                    if (c == ' ' || c == '_' || c == '-')
                    {
                        return (upperNext: true, sb);
                    }

                    if (upperNext.HasValue)
                    {
                        sb.Append(upperNext.Value ?
                            c.ToString().ToUpperInvariant() :
                            c.ToString().ToLowerInvariant()
                            );
                    }
                    else
                    {
                        sb.Append(c);
                    }

                    return (null, sb);
                });

                output.Write(aggregated.result.ToString());
            }
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
            if (!string.IsNullOrEmpty(toCase))
            {
                var chars = toCase.Trim().ToArray();
                var seed = (previousUnderscore: true, previousUpper: true, result: new StringBuilder());
                var aggregated = chars.Aggregate(seed, (s, c) =>
                {
                    Trace.WriteLine($"{s}");
                    var (previousUnderscore, previousUpper, sb) = s;

                    //if ()
                    //{
                    //    return (true, previousUnderscore, sb);
                    //}

                    if ((c == '_' || c == ' ' || c == '-'))
                    {
                        if (!previousUnderscore)
                            sb.Append('_');
                        return (true, false, sb);
                    }

                    var isUpper = char.IsUpper(c);

                    if (isUpper && !previousUnderscore && !previousUpper)
                    {
                        sb.Append('_');
                    }

                    sb.Append(c.ToString().ToLower());

                    return (false, isUpper, sb);
                });

                output.Write(aggregated.result.ToString());
            }
        }
    }
}
