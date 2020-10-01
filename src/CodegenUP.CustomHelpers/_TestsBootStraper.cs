using CodegenUP;
using CodegenUP.CustomHandlebars;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CodegenUP.CustomHelpers
{
    public class TestsBootStraper
    {
        [Theory]
        [MemberData(nameof(HelpersTests_Data))]
        public void HelpersTests_Json(IHelper helper, string json, string template, string expectedOutput)
          => HelpersTesterHelper.GetHelperResultFromJToken(helper, json, template).ShouldBe(expectedOutput);

        public static IEnumerable<object[]> HelpersTests_Data() => HelpersTesterHelper.GetHelpersTestsData(HandlebarsConfigurationHelper.GetHelpersFromAssembly(typeof(TestsBootStraper).Assembly));
    }
}
