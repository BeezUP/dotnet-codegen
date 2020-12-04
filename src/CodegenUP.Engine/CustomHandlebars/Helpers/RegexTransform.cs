using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CodegenUP.CustomHandlebars.Helpers
{
#if DEBUG
    [HandlebarsHelperSpecification("{ test: \"/user/{username}\" }", "{{regex_transform test '{(.*)}', ':$1'}}", "/user/:username")]
    [HandlebarsHelperSpecification("{ test: \"/user/{username}\" }", "{{regex_transform test '{(.*)}', '<$1>'}}", "/user/<username>")]
#endif
    public class RegexTransform : SimpleStandardHelperBase<object, string, string, string>
    {
        public RegexTransform() : base("regex_transform") { }

        public override void HelperFunction(TextWriter output, object context, string arg, string regexPattern, string regexReplacement, object[] otherArguments)
        {
            var replaced = Regex.Replace(arg, regexPattern, regexReplacement);
            output.WriteSafeString(replaced);
        }
    }
}
