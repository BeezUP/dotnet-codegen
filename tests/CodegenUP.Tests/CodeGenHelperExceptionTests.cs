using CodegenUP.CustomHandlebars;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CodegenUP.Tests
{
    public class CodeGenHelperExceptionTests
    {
        [Fact]
        public void Exception_should_provide_the_helper_name_in_the_message()
        {
            new CodeGenHelperException("TestHelper", "i'm not OK!").Message.ShouldBe("TestHelper: i'm not OK!");
            new CodeGenHelperException("TestHelper2", "i'm not OK!", new Exception()).Message.ShouldBe("TestHelper2: i'm not OK!");
        }
    }
}
