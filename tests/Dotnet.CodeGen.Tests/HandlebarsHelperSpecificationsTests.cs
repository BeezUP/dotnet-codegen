using Dotnet.CodeGen.CustomHandlebars;
using HandlebarsDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
        public void HelpersTests_Json(IHelper helper, string json, string template, string expectedOutput)
            => HelpersTesterHelper.GetHelperResultFromJToken(helper, json, template).ShouldBe(expectedOutput);

        [Theory]
        [MemberData(nameof(HelpersTests_Data))]
        public void HelpersTests_Expando(IHelper helper, string json, string template, string expectedOutput)
            => HelpersTesterHelper.GetHelperResultFromExpando(helper, json, template).ShouldBe(expectedOutput);

        public static IEnumerable<object[]> HelpersTests_Data() => HelpersTesterHelper.GetHelpersTestsData(HandlebarsConfigurationHelper.DefaultHelpers);
    }
}

#endif