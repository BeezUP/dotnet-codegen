using HandlebarsDotNet;
using System;
using System.IO;
using System.Linq;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// Split the string & return the last chunk
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{ '$ref' : '#/parameters/myDataType'}", "{{split_get_last ./$ref '/' }}", "myDataType")]
#endif
    public class SplitGetLast : SimpleStandardHelperBase<object, string, string>
    {
        public SplitGetLast() : base("split_get_last") { }

        public override void HelperFunction(TextWriter output, object context, string argument, string splitter, object[] arguments)
        {
            output.Write(argument?.Split(new[] { splitter }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "");
        }
    }

    /// <summary>
    /// Split the string & return the first chunk
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{ '$ref' : '/myDataType/parameters/'}", "{{split_get_first ./$ref '/' }}", "myDataType")]
#endif
    public class SplitGetFirst : SimpleStandardHelperBase<object, string, string>
    {
        public SplitGetFirst() : base("split_get_first") { }

        public override void HelperFunction(TextWriter output, object context, string argument, string splitter, object[] arguments)
        {
            output.Write(argument?.Split(new[] { splitter }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "");
        }
    }
}
