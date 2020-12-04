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
        [InlineData("hello\\world", "HelloWorld")]
        [InlineData("hello|world", "HelloWorld")]
        [InlineData("hello/world", "HelloWorld")]
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
        [InlineData("Hello\\world", "helloWorld")]
        [InlineData("Hello||world", "helloWorld")]
        [InlineData("Hello/world", "helloWorld")]
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
        [InlineData("hello/World", "hello_world")]
        [InlineData("hello|World", "hello_world")]
        [InlineData("hello\\World", "hello_world")]
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


        [Theory]
        [InlineData(" ", "\n")]
        [InlineData(" |do not < remove please >| ", "|do not < remove please >|\n")]
        [InlineData(" \n ", "\n")]
        [InlineData("\n ", "\n")]
        [InlineData("\n", "\n")]
        [InlineData(" \r\n ", "\n")]
        [InlineData("\r\n", "\n")]
        [InlineData(" test", "test\n")]
        [InlineData(" a \n z ", "az\n")]
        [InlineData("a\n z", "az\n")]
        [InlineData("a\nz", "az\n")]
        [InlineData("a \r\n z", "az\n")]
        [InlineData("a \r\n \r\n \r\nz", "az\n")]
        [InlineData("test\r\n\r\n\r\ntest", "testtest\n")]
        [InlineData("", "\n")]
        public void OnOneLine(string str, string expected)
        {
            StringHelpers.OnOneLine(str).ShouldBe(expected);
        }

        [Theory]
        [InlineData("test\r\n\r\n\r\ntest", 0, true, "testtest\n")]
        [InlineData("test\r\n\r\n\r\ntest", 0, false, "testtest")]
        [InlineData("test", 5, false, "     test")]
        public void OnOneLineWithOptionalParameters(string str, int? indent, bool? lineBreak, string expected)
        {
            StringHelpers.OnOneLine(str, indent, lineBreak).ShouldBe(expected);
        }
    }
}
