using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars
{
    public abstract class SimpleBlockHelperBase : HelperBase
    {
        public SimpleBlockHelperBase(string name) : base(name) { }
        public abstract HandlebarsBlockHelper Helper { get; }

        public override void Setup(HandlebarsConfiguration configuration)
        {
            configuration.BlockHelpers.Add(Name, Helper);
        }
    }
}
