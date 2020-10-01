using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars
{
    public abstract class SimpleStandardHelperBase : HelperBase
    {
        public SimpleStandardHelperBase(string name) : base(name) { }
        public abstract HandlebarsHelper Helper { get; }

        public override void Setup(HandlebarsConfiguration configuration)
        {
            configuration.Helpers.Add(Name, Helper);
        }
    }
}
