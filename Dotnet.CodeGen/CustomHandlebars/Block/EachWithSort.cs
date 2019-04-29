using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars.Block
{
    /// <summary>
    /// 
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification(GLOBAL_SPECS.SWAGGER_SAMPLE, "{{#each_with_sort parameters}} {{@key}} {{/each_with_sort}}", "AA")]
#endif
    public class EachWithSort : BlockHelperBase
    {
        public EachWithSort() : base("each_with_sort") { }

        public override HandlebarsBlockHelper Helper =>
            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
            {
                EnsureArgumentsCount(arguments, 1);



                var children = ((JObject)arguments.First()).Children().Select(x => (JProperty)x).ToArray();

                var ret = new JObject(children.OrderBy(x => x.Name));

                options.Template(output, ret);
            };
    }
}
