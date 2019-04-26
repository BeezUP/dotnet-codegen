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
                throw new CodeGenHelperException($"{Name} needs exactly {count} arguments.");
        }

        protected void EnsureArgumentsCountMax(object[] arguments, int count)
        {
            var argumentsCount = arguments?.Length ?? 0;
            if (argumentsCount > count)
                throw new CodeGenHelperException($"{Name} needs {count} arguments maximum.");
        }
        protected void EnsureArgumentsCountMin(object[] arguments, int count)
        {
            var argumentsCount = arguments?.Length ?? 0;
            if (argumentsCount < count)
                throw new CodeGenHelperException($"{Name} needs {count} arguments minimum.");
        }

        protected JContainer GetJContainerContext(object context)
        {
            return context as JContainer
                ?? throw new CodeGenHelperException($"Context must be a valid json container for helper {Name} to work.");
        }

        protected JArray GetArgumentAsArray(object[] arguments, int argumentIndex)
        {
            return arguments[argumentIndex] as JArray
                ?? throw new CodeGenHelperException($"Argument {argumentIndex} shouldbe an array but is of type '{arguments[argumentIndex]?.GetType().Name}'.");
        }

        protected string GetArgumentStringValue(object[] arguments, int argumentIndex)
        {
            return arguments[argumentIndex]?.ToString();
        }

        protected string GetStringValue(object obj)
        {
            if (obj is string s) return s;

            if (obj is JValue jToken)
            {
                if (jToken.Type == JTokenType.String) return jToken.Value as string;
                else jToken.Value?.ToString();
            }

            throw new CodeGenHelperException($"No string value could be extracted from type {obj?.GetType().Name}");
        }
    }
}
