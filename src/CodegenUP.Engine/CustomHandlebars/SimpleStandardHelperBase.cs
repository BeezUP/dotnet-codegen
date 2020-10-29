using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodegenUP.CustomHandlebars
{
    public abstract class SimpleStandardHelperBase<TContext, TArgument1, TArgument2, TArgument3> : SimpleStandardHelperBase
    {
        public SimpleStandardHelperBase(string name) : base(name) { }

        public override void Helper(TextWriter output, object context, object[] arguments)
        {
            var (ok, ctx) = ObjectTo<TContext>(context);
            if (!ok) throw new CodeGenHelperException(Name, $"Unable to get the context as a {typeof(TContext).Name}");
            if (!TryGetArgumentAs<TArgument1>(arguments, 0, out var argument1))
                throw new CodeGenHelperException(Name, $"Unable to get the first argument as a {typeof(TArgument1).Name}");
            if (!TryGetArgumentAs<TArgument2>(arguments, 1, out var argument2))
                throw new CodeGenHelperException(Name, $"Unable to get the second argument as a {typeof(TArgument2).Name}");
            if (!TryGetArgumentAs<TArgument3>(arguments, 2, out var argument3))
                throw new CodeGenHelperException(Name, $"Unable to get the third argument as a {typeof(TArgument3).Name}");

            HelperFunction(output, ctx, argument1, argument2, argument3, arguments.Skip(3).ToArray());
        }

        public abstract void HelperFunction(TextWriter output, TContext context, TArgument1 argument1, TArgument2 argument2, TArgument3 argument3, object[] otherArguments);
    }

    public abstract class SimpleStandardHelperBase<TContext, TArgument1, TArgument2> : SimpleStandardHelperBase
    {
        public SimpleStandardHelperBase(string name) : base(name) { }

        public override void Helper(TextWriter output, object context, object[] arguments)
        {
            var (ok, ctx) = ObjectTo<TContext>(context);
            if (!ok) throw new CodeGenHelperException(Name, $"Unable to get the context as a {typeof(TContext).Name}");
            if (!TryGetArgumentAs<TArgument1>(arguments, 0, out var argument1))
                throw new CodeGenHelperException(Name, $"Unable to get the first argument as a {typeof(TArgument1).Name}");
            if (!TryGetArgumentAs<TArgument2>(arguments, 1, out var argument2))
                throw new CodeGenHelperException(Name, $"Unable to get the second argument as a {typeof(TArgument2).Name}");

            HelperFunction(output, ctx, argument1, argument2, arguments.Skip(2).ToArray());
        }

        public abstract void HelperFunction(TextWriter output, TContext context, TArgument1 argument1, TArgument2 argument2, object[] otherArguments);
    }

    public abstract class SimpleStandardHelperBase<TContext, TArgument1> : SimpleStandardHelperBase
    {
        public SimpleStandardHelperBase(string name) : base(name) { }

        public override void Helper(TextWriter output, object context, object[] arguments)
        {
            var (ok, ctx) = ObjectTo<TContext>(context);
            if (!ok) throw new CodeGenHelperException(Name, $"Unable to get the context as a {typeof(TContext).Name}");
            if (!TryGetArgumentAs<TArgument1>(arguments, 0, out var argument1))
                throw new CodeGenHelperException(Name, $"Unable to get the first argument as a {typeof(TArgument1).Name}");

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

        public abstract void HelperFunction(TextWriter output, TContext context, object[] otherArguments);
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
