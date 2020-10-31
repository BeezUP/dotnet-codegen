using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// Output some debug informations about the context and arguments
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{dbg 'test'}}", "JObject: {}\nString: test\n")]
#endif
    public class Debug : SimpleStandardHelperBase
    {
        public Debug() : base("dbg") { }

        public override void Helper(TextWriter output, object context, object[] arguments)
        {
            Output(output, context);
            for (int i = 0; i < arguments.Length; i++)
            {
                Output(output, arguments[i]);
            }
        }

        void Output(TextWriter output, object obj)
        {
            if (obj == null) return;
            output.Write($"{obj.GetType().Name}: {obj}");
            output.Write("\n");
        }
    }
}
