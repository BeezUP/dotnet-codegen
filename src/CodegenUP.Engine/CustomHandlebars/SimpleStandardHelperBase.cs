using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodegenUP.CustomHandlebars
{
    public abstract class SimpleStandardHelperBase : HelperBase
    {
        public SimpleStandardHelperBase(string name) : base(name) { }
        public abstract void Helper(TextWriter output, object context, object[] arguments);
        public override void Setup(HandlebarsConfiguration configuration)
        {
            configuration.Helpers.Add(Name, Helper);
        }
    }
}
