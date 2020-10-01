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


        [Fact]
        public void ShouldBeAbleToGetJsonFromYamlFile()
        {
            var att = new HandlebarsHelperSpecificationAttribute("_samples/simple_files/Merge1_expected.yaml", "osef", "osef");
            var json = att.GetJsonDocument();
            JToken.Parse(json);
        }

        [Fact]
        public void ShouldBeAbleToGetJsonFromJsonFile()
        {
            var att = new HandlebarsHelperSpecificationAttribute("_samples/simple_files/Merge1_rest.json", "osef", "osef");
            var json = att.GetJsonDocument();
            JToken.Parse(json);
        }

        [Fact]
        public void ShouldBeAbleToGetJsonTheAttributeItSelf()
        {
            var att = new HandlebarsHelperSpecificationAttribute("{}", "osef", "osef");
            var json = att.GetJsonDocument();
            json.ShouldBe("{}");
            var token = JToken.Parse(json);
            token.Type.ShouldBe(JTokenType.Object);
        }
    }
}

#endif