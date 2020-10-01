using HandlebarsDotNet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodegenUP.CustomHandlebars
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

        /// <summary>
        /// Ensure the arguments count is exactly `count`
        /// </summary>
        protected void EnsureArgumentsCount(object[] arguments, int count)
        {
            var argumentsCount = arguments?.Length ?? 0;
            if (argumentsCount != count)
                throw new CodeGenHelperException($"{Name}: needs exactly {count} arguments.");
        }
        /// <summary>
        /// Ensure the arguments count is a maximum of `count`
        /// </summary>
        protected void EnsureArgumentsCountMax(object[] arguments, int count)
        {
            var argumentsCount = arguments?.Length ?? 0;
            if (argumentsCount > count)
                throw new CodeGenHelperException($"{Name}: needs {count} arguments maximum.");
        }
        /// <summary>
        /// Ensure the arguments count is a minimum of `count`
        /// </summary>
        protected void EnsureArgumentsCountMin(object[] arguments, int count)
        {
            var argumentsCount = arguments?.Length ?? 0;
            if (argumentsCount < count)
                throw new CodeGenHelperException($"{Name}: needs {count} arguments minimum.");
        }
        protected void EnsureArgumentIndexExists(object[] arguments, int argumentIndex)
        {
            var argumentsCount = arguments?.Length ?? 0;
            if (argumentIndex >= argumentsCount)
                throw new CodeGenHelperException($"{Name}: the argument at index {argumentIndex} is needed.");
        }

        /// <summary>
        /// Returns the argument at `argumentIndex` or throw an exception if the argument does not exits
        /// </summary>
        protected object GetArgumentOrThrow(object[] arguments, int argumentIndex)
        {
            EnsureArgumentIndexExists(arguments, argumentIndex);
            return arguments[argumentIndex];
        }


        protected T GetArgumentAs<T>(object[] arguments, int argumentIndex)
        {
            object arg = GetArgumentOrThrow(arguments, argumentIndex);
            var (ok, result) = ObjectTo<T>(arg);
            if (!ok)
                throw new CodeGenHelperException($"{Name}: Couldn't convert argument at index {argumentIndex} as {typeof(T).Name}");
            return result;
        }

        protected bool TryGetArgumentAs<T>(object[] arguments, int argumentIndex, out T? result)
            where T : class
        {
            if (argumentIndex >= arguments.Length)
            {
                result = default;
                return false;
            }

            object arg = arguments[argumentIndex];
            var (ok, res) = ObjectTo<T>(arg);
            if (!ok)
            {
                result = default;
                return false;
            }
            else
            {
                result = res;
                return true;
            }
        }

        protected bool TryGetArgumentAs<T>(object[] arguments, int argumentIndex, out T? result)
            where T : struct
        {
            if (argumentIndex >= arguments.Length)
            {
                result = default;
                return false;
            }

            object arg = arguments[argumentIndex];
            var (ok, res) = ObjectTo<T>(arg);
            if (!ok)
            {
                result = default;
                return false;
            }
            else
            {
                result = res;
                return true;
            }
        }

        protected object[] GetArgumentAsArray(object[] arguments, int argumentIndex)
        {
            object arg = arguments[argumentIndex];
            return (arg as IEnumerable)?.Cast<object>().ToArray()
                ?? throw new CodeGenHelperException($"Argument {argumentIndex} should be enumerable but is of type '{arg?.GetType().Name}'.");
        }

        protected string? GetStringValue(object obj)
        {
            if (obj is string s) return s;

            if (obj is JValue jToken)
            {
                if (jToken.Type == JTokenType.String) return jToken.Value as string;
                else jToken.Value?.ToString();
            }

            throw new CodeGenHelperException($"No string value could be extracted from type {obj?.GetType().Name}");
        }

        protected (bool ok, T result) ObjectTo<T>(object o)
        {
            if (o is T t) return (true, t);

            if (typeof(T) == typeof(string)) return (true, (T)(object)o.ToString());

            if (o is JToken jToken)
            {
                return (true, jToken.ToObject<T>());
            }

            return (false, default);
        }
    }
}
