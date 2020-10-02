using CodegenUP.CustomHandlebars;
using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Xunit;

namespace CodegenUP.Tests.CustomHandlebars
{
    public class HelperBaseTests : HelperBase
    {
        public HelperBaseTests() : base("unit_tests") { }

        public override void Setup(HandlebarsConfiguration configuration)
        {
            // do nothing
        }

        [Fact]
        public void Test()
        {

        }

        [Fact]
        public void ObjectToTests()
        {
            ObjectTo<object>("").result.ShouldBe((object)"");
            ObjectTo<object>(42).result.ShouldBe((object)42);
            ObjectTo<string>("").result.ShouldBe("");
            ObjectTo<string>(JToken.Parse("{}")).result.ShouldBe("{}");
            ObjectTo<int>(JToken.Parse("1")).result.ShouldBe(1);

            {
                var (ok, result) = ObjectTo<ExpandoObject>(JToken.Parse("{ value: 42 }"));
                ok.ShouldBeTrue();
                result.ShouldBeOfType<ExpandoObject>();
                dynamic dyn = result;
                ShouldBeTestExtensions.ShouldBe(dyn.value, 42);
            }

            ObjectTo<JObject>(JToken.Parse("{ value: 42 }")).result.Value<int>("value").ShouldBe(42);
            ObjectTo<JToken>(JToken.Parse("{ value: 42 }")).result.Value<int>("value").ShouldBe(42);
            ObjectTo<JValue>(JToken.Parse("{ value: 42 }")).ok.ShouldBeFalse();
            ObjectTo<MyClass>(JToken.Parse("{ i: 42, s:\"ok\" }")).result.S.ShouldBe("ok");
            ObjectTo<MyClass>(JToken.Parse("{ i: 42, s:\"ok\" }")).result.S.ShouldBe("ok");
            ObjectTo<MyClass>(JToken.Parse("{ i: 42 }")).result.S.ShouldBeNull();
            ObjectTo<MyClass[]>(JToken.Parse("[{ i: 42, s:\"ok\" }, { i: 42, s:\"ok\" }]")).ok.ShouldBeTrue();
            ObjectTo<MyClass[]>(JToken.Parse("[{ i: 42, s:\"ok\" }, { i: 42, s:\"ok\" }]")).result.Length.ShouldBe(2);
        }

        class MyClass
        {
            public int I { get; set; }
            public string? S { get; set; }
        }
    }
}
