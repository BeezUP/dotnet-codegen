using CodegenUP.CustomHandlebars;
using HandlebarsDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace CodegenUP
{
    public static class HelpersTesterHelper
    {
        public static IEnumerable<object[]> GetHelpersTestsData(IEnumerable<IHelper> helpers)
        {
            foreach (var helper in helpers)
                foreach (var att in helper.GetType().GetCustomAttributes(typeof(HandlebarsHelperSpecificationAttribute), false).Cast<HandlebarsHelperSpecificationAttribute>())
                {
                    yield return new object[] { helper, att.GetJsonDocument(), att.Template, att.ExpectedOutput };
                }
        }

        public static string GetHelperResultFromExpando(IHelper helper, string json, string template)
        {
            var configuration = new HandlebarsConfiguration();
            helper.Setup(configuration);
            var handleBar = Handlebars.Create(configuration);

            var hb = handleBar.Compile(template);
            var obj = JsonHelper.GetDynamicObjectFromJson(JToken.Parse(json));
            var output = hb.Invoke(obj);
            return output;
        }

        public static string GetHelperResultFromJToken(IHelper helper, string json, string template)
        {
            var configuration = new HandlebarsConfiguration();
            helper.Setup(configuration);
            var handleBar = Handlebars.Create(configuration);

            var hb = handleBar.Compile(template);
            var output = hb.Invoke(JToken.Parse(json));
            return output;
        }
    }
}
