using Dotnet.CodeGen.CustomHandlebars;
using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

#if DEBUG

namespace Dotnet.CodeGen.Tests
{
    public class HandlebarsHelperSpecificationsTests
    {
        [Theory]
        [MemberData(nameof(HelpersTests_Data))]
        public void HelpersTests(IHelper helper, string json, string template, string expectedOutput)
        {
            var configuration = new HandlebarsConfiguration();
            helper.Setup(configuration);
            var handleBar = Handlebars.Create(configuration);

            var hb = handleBar.Compile(template);
            var output = hb.Invoke(JToken.Parse(json));
            output.ShouldBe(expectedOutput);
        }

        public static IEnumerable<object[]> HelpersTests_Data()
        {
            foreach (var helper in HandlebarsConfigurationHelper.Helpers)
                foreach (var att in helper.GetType().GetCustomAttributes(typeof(HandlebarsHelperSpecificationAttribute), false).Cast<HandlebarsHelperSpecificationAttribute>())
                {
                    yield return new object[] { helper, att.Json, att.Template, att.ExpectedOutput };
                }
        }
    }
}

#endif