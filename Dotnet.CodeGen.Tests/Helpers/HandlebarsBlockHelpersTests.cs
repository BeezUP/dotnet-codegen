using Dotnet.CodeGen.CustomHandlebars;
using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Dotnet.CodeGen.Tests.Helpers
{
    public class HandlebarsBlockHelpersTests
    {
        [Fact]
        public void Test()
        {
//            var handleBar = Handlebars.Create(new HandlebarsConfiguration
//            {
//                BlockHelpers = {
//                    { "my_helper", HandlebarsBlockHelpers.IsEnum }
//                }
//            });

//            var output = handleBar
//                .Compile(@"{{#my_helper this}}{{/my_helper}}")
//                (JObject.Parse("{}"))
//                ;

////"{{#each order_by values property}}"

//            output.ShouldBe("");
        }
    }
}
