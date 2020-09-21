using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars
{
    public abstract class HelperBase : IHelper
    {
        public HelperBase(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public override string ToString() => Name;

        public abstract void Setup(HandlebarsConfiguration configuration);


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

        protected object[] GetArgumentAsArray(object[] arguments, int argumentIndex)
        {
            object arg = arguments[argumentIndex];
            return (arg as IEnumerable)?.Cast<object>().ToArray()
                ?? throw new CodeGenHelperException($"Argument {argumentIndex} should be enumerable but is of type '{arg?.GetType().Name}'.");
        }

        protected string GetArgumentStringValue(object[] arguments, int argumentIndex)
        {
            return arguments[argumentIndex]?.ToString();
        }

        protected char GetArgumentCharValue(object[] arguments, int argumentIndex)
        {
            if (argumentIndex >= arguments.Length || arguments[argumentIndex] == null) throw new CodeGenHelperException($"{Name} needs an argument at position {argumentIndex}.");

            if (!char.TryParse(arguments[argumentIndex].ToString(), out var @char)) throw new CodeGenHelperException($"Argument {arguments[argumentIndex]} should be a char.");

            return @char;
        }

        protected char GetArgumentCharValueOrDefault(object[] arguments, int argumentIndex, char defaultValue = default)
        {
            if (argumentIndex >= arguments.Length || arguments[argumentIndex] == null)
            {
                if (defaultValue == default) throw new CodeGenHelperException($"{Name} needs an argument at position {argumentIndex}.");

                return defaultValue;
            }

            if (!char.TryParse(arguments[argumentIndex].ToString(), out var @char)) throw new CodeGenHelperException($"Argument {arguments[argumentIndex]} should be a char.");

            return @char;
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
