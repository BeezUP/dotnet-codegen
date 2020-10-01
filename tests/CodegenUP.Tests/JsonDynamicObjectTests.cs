using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace CodegenUP.Tests
{
    //public class JsonDynamicObjectTests
    //{
    //    private readonly ITestOutputHelper _output;

    //    public JsonDynamicObjectTests(ITestOutputHelper output)
    //    {
    //        _output = output;
    //    }

    //    [Fact]
    //    public void JObjectBehaviors()
    //    {
    //        var jobj = JObject.Parse("{ \"test\": 'value'}");
    //        var o = new JsonDynamicObject(jobj);
    //        o.GetDynamicMemberNames().ShouldBe(new[] { "test" });

    //        dynamic dyn = o;
    //        Should.Throw<RuntimeBinderException>(() => { var t = dyn.not_existing; });

    //        ((object)dyn.test).ShouldBeOfType<JsonDynamicObject>();

    //        _output.WriteLine(dyn.test.ToString());
    //    }
    //}
}
