using HandlebarsDotNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;

namespace CodegenUP.CustomHandlebars
{
    public abstract class SimpleBlockHelperBase<TContext> : SimpleBlockHelperBase
    {
        public SimpleBlockHelperBase(string name) : base(name) { }

        public override void Helper(TextWriter output, HelperOptions options, object context, object[] arguments)
        {
            var (ok, result) = ObjectTo<TContext>(context);
            if (!ok) throw new CodeGenHelperException($"Unable to get the context as a {typeof(TContext)}");
            HelperFunction(output, options, result, arguments);
        }

        public abstract void HelperFunction(TextWriter output, HelperOptions options, TContext context, object[] arguments);
    }

    public abstract class SimpleBlockHelperBase : HelperBase
    {
        public SimpleBlockHelperBase(string name) : base(name) { }
        public abstract void Helper(TextWriter output, HelperOptions options, object context, object[] arguments);
        public override void Setup(HandlebarsConfiguration configuration)
        {
            configuration.BlockHelpers.Add(Name, Helper);
        }
    }
}
