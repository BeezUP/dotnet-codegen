using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CodegenUP.Tests
{
    public class StringHelpersTests
    {
        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("42", "42")]
        [InlineData("HELLO", "HELLO")]
        [InlineData("HelloWorld", "HelloWorld")]
        [InlineData("hello", "Hello")]
        [InlineData("heLlo wOrld", "HeLloWOrld")]
        [InlineData("hello_world", "HelloWorld")]
        [InlineData("hello-world", "HelloWorld")]
        [InlineData("hello-WORLD", "HelloWORLD")]
        public void ToPascalCase(string toCase, string expected)
        {
            StringHelpers.ToPascalCase(toCase).ShouldBe(expected);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("42", "42")]
        [InlineData("HELLO", "hELLO")]
        [InlineData("hello", "hello")]
        [InlineData("heLlo wOrld", "heLloWOrld")]
        [InlineData("hello_world", "helloWorld")]
        [InlineData("hello-world", "helloWorld")]
        [InlineData("hello-WORLD", "helloWORLD")]
        [InlineData("HelloWorld", "helloWorld")]
        public void ToCamelCase(string toCase, string expected)
        {
            StringHelpers.ToCamelCase(toCase).ShouldBe(expected);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("42", "42")]
        [InlineData("HELLO", "hello")]
        [InlineData("hello world", "hello_world")]
        [InlineData("hello_world", "hello_world")]
        [InlineData("hello-world", "hello_world")]
        [InlineData("hello--world", "hello_world")]
        [InlineData("hello__World", "hello_world")]
        [InlineData("hello-World", "hello_world")]
        [InlineData("hello _ world", "hello_world")]
        [InlineData("hello - world", "hello_world")]
        [InlineData("HelloWorld", "hello_world")]
        [InlineData("hello _WORLD", "hello_world")]
        [InlineData(" HELLO", "hello")]
        [InlineData("Hello ", "hello")]
        [InlineData("2Hello2 ", "2_hello2")]
        [InlineData("HelloWorld_42LongName", "hello_world_42_long_name")]
        public void ToSnakeCase(string toCase, string expected)
        {
            StringHelpers.ToSnakeCase(toCase).ShouldBe(expected);
        }
    }
}
