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
                throw new CodeGenHelperException(Name, $"needs exactly {count} arguments.");
        }
        /// <summary>
        /// Ensure the arguments count is a maximum of `count`
        /// </summary>
        protected void EnsureArgumentsCountMax(object[] arguments, int count)
        {
            var argumentsCount = arguments?.Length ?? 0;
            if (argumentsCount > count)
                throw new CodeGenHelperException(Name, $"needs {count} arguments maximum.");
        }
        /// <summary>
        /// Ensure the arguments count is a minimum of `count`
        /// </summary>
        protected void EnsureArgumentsCountMin(object[] arguments, int count)
        {
            var argumentsCount = arguments?.Length ?? 0;
            if (argumentsCount < count)
                throw new CodeGenHelperException(Name, $"needs {count} arguments minimum.");
        }
        protected void EnsureArgumentIndexExists(object[] arguments, int argumentIndex)
        {
            var argumentsCount = arguments?.Length ?? 0;
            if (argumentIndex >= argumentsCount)
                throw new CodeGenHelperException(Name, $"the argument at index {argumentIndex} is needed.");
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
                throw new CodeGenHelperException(Name, $"Couldn't convert argument at index {argumentIndex} as {typeof(T).Name}");
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
                ?? throw new CodeGenHelperException(Name, $"Argument {argumentIndex} should be enumerable but is of type '{arg?.GetType().Name}'.");
        }

        protected (bool ok, T result) ObjectTo<T>(object o)
        {
            return TryObjectToType(o, typeof(T), out var result) && result != null
                ? (true, (T)result)
                : (false, default);
        }

        public class NULL
        {
            public static readonly NULL Value = new NULL();
            private NULL() { }
            public override string ToString() => "Null";
        }
        //private static readonly Type JTOKEN_TYPE = typeof(JToken);
        private static readonly Type STRING_TYPE = typeof(string);

        private static bool TryObjectToType(object o, Type expectedType, out object result)
        {
            result = NULL.Value;

            if (o == null) return false;

            var oType = o.GetType();

            if (expectedType.IsAssignableFrom(oType))
            {
                result = o;
                return true;
            }

            if (expectedType == STRING_TYPE)
            {
                result = o.ToString();
                return true;
            }

            if (expectedType.IsArray)
            {
                var objects = (o as IEnumerable)?.Cast<object>();
                if (objects == null) return false;
                var elementType = expectedType.GetElementType();
                var results = objects.Select(o =>
                {
                    var ok = TryObjectToType(o, elementType, out var elementResult);
                    return (ok, elementResult);
                }).ToArray();

                if (!results.All(r => r.ok)) return false;
                var array = Array.CreateInstance(elementType, results.Length);
                for (int i = 0; i < results.Length; i++)
                {
                    array.SetValue(results[i].elementResult, i);
                }

                result = array;
                return true;
            }

            if (o is JToken jToken)
            {
                try
                {
                    var obj = jToken.ToObject(expectedType);
                    if (obj == null || obj.GetType() != expectedType) return false;
                    result = obj;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }
    }
}
