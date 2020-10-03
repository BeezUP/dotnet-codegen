using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodegenUP.CustomHandlebars
{
    public abstract class SimpleStandardHelperBase<TContext, TArgument1, TArgument2> : SimpleStandardHelperBase
    {
        public SimpleStandardHelperBase(string name) : base(name) { }

        public override void Helper(TextWriter output, object context, object[] arguments)
        {
            EnsureArgumentsCountMin(arguments, 2);

            var (ok, ctx) = ObjectTo<TContext>(context);
            if (!ok) throw new CodeGenHelperException(Name, $"Unable to get the context as a {typeof(TContext).Name}");
            var (ok1, argument1) = ObjectTo<TArgument1>(arguments[0]);
            if (!ok1) throw new CodeGenHelperException(Name, $"Unable to get the first argument as a {typeof(TArgument1).Name}");
            var (ok2, argument2) = ObjectTo<TArgument2>(arguments[1]);
            if (!ok2) throw new CodeGenHelperException(Name, $"Unable to get the second argument as a {typeof(TArgument2).Name}");

            HelperFunction(output, ctx, argument1, argument2, arguments.Skip(2).ToArray());
        }

        public abstract void HelperFunction(TextWriter output, TContext context, TArgument1 argument1, TArgument2 argument2, object[] otherArguments);
    }

    public abstract class SimpleStandardHelperBase<TContext, TArgument1> : SimpleStandardHelperBase
    {
        public SimpleStandardHelperBase(string name) : base(name) { }

        public override void Helper(TextWriter output, object context, object[] arguments)
        {
            EnsureArgumentsCountMin(arguments, 1);

            var (ok, ctx) = ObjectTo<TContext>(context);
            if (!ok) throw new CodeGenHelperException(Name, $"Unable to get the context as a {typeof(TContext).Name}");
            var (ok1, argument1) = ObjectTo<TArgument1>(arguments[0]);
            if (!ok1) throw new CodeGenHelperException(Name, $"Unable to get the first argument as a {typeof(TArgument1).Name}");

            HelperFunction(output, ctx, argument1, arguments.Skip(1).ToArray());
        }

        public abstract void HelperFunction(TextWriter output, TContext context, TArgument1 argument1, object[] otherArguments);
    }

    public abstract class SimpleStandardHelperBase<TContext> : SimpleStandardHelperBase
    {
        public SimpleStandardHelperBase(string name) : base(name) { }

        public override void Helper(TextWriter output, object context, object[] arguments)
        {
            var (ok, ctx) = ObjectTo<TContext>(context);
            if (!ok) throw new CodeGenHelperException(Name, $"Unable to get the context as a {typeof(TContext)}");
            HelperFunction(output, ctx, arguments);
        }

        public abstract void HelperFunction(TextWriter output, TContext context, object[] arguments);
    }


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
