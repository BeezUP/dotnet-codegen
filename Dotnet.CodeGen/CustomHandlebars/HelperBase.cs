using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars
{
    public class HelperBase
    {
        public HelperBase(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString() => Name;


        protected void EnsureArgumentsCount(object[] arguments, int count)
        {
            var argumentsCount = arguments?.Length ?? 0;
            if (argumentsCount != count)
                throw new ArgumentException($"{Name} needs exactly {count} arguments.");
        }

        protected void EnsureArgumentsCountMax(object[] arguments, int count)
        {
            var argumentsCount = arguments?.Length ?? 0;
            if (argumentsCount > count)
                throw new ArgumentException($"{Name} needs {count} arguments maximum.");
        }
        protected void EnsureArgumentsCountMin(object[] arguments, int count)
        {
            var argumentsCount = arguments?.Length ?? 0;
            if (argumentsCount < count)
                throw new ArgumentException($"{Name} needs {count} arguments minimum.");
        }

        protected JContainer GetJContainerContext(object context)
        {
            return context as JContainer ?? throw new ArgumentException($"Context must be a valid json container for helper {Name} to work.");
        }
    }
}
