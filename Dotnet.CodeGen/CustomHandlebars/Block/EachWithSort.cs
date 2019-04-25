using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars.Block
{
    //[HandlebarsHelperSpecification("{ test: 'AA' }", "{{uppercase_first_letter test}}", "AA")]
    public class EachWithSort : BlockHelperBase
    {
        public EachWithSort() :
            base(
                "each_with_sort",
                () => (TextWriter output, HelperOptions options, dynamic context, object[] arguments) =>
                {
                    var children = ((JObject)arguments.First()).Children().Select(x => (JProperty)x).ToArray();

                    var ret = new JObject(children.OrderBy(x => x.Name));

                    options.Template(output, ret);
                })
        {
        }
    }
}
