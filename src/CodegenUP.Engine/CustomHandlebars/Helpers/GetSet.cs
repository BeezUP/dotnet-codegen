using HandlebarsDotNet;
using System.Collections.Generic;
using System.IO;

namespace CodegenUP.CustomHandlebars.Helpers
{
    /// <summary>
    /// Give the ability to set a variable in template.
    /// It's like a key/value store : {{set 'key' value}} {{get 'key'}}
    /// </summary>
#if DEBUG
    [HandlebarsHelperSpecification("{}", "{{set 'key', 'value'}}{{get 'key'}}", "value")]
    [HandlebarsHelperSpecification("{ key: 'value' }", "{{set 'k', . }}{{#with_get 'k'}}{{key}}{{/with_get}}", "value")]
    [HandlebarsHelperSpecification("{ key: 'value' }", "{{#with_set 'key', .key }}{{get 'key'}}{{/with_set}}{{get 'key'}}", "value")]
    [HandlebarsHelperSpecification("{}", "{{set 'key', '42' }}{{get 'key'}}{{clear 'key'}}{{get 'key'}}", "42")]
#endif
    public class GetSet : HelperBase
    {
        readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public GetSet() : base("get/set") { }

        public HandlebarsHelper SetHelper =>
            (TextWriter output, object context, object[] arguments) =>
            {
                EnsureArgumentsCount(arguments, 2);

                var key = GetArgumentStringValue(arguments, 0) ?? throw new CodeGenHelperException($"Cannot set a value with no key");
                var value = arguments[1];
                _values[key] = value;
            };

        public HandlebarsBlockHelper WithSetHelper =>
            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
            {
                EnsureArgumentsCount(arguments, 2);

                var key = GetArgumentStringValue(arguments, 0) ?? throw new CodeGenHelperException($"Cannot set a value with no key");
                var value = arguments[1];
                _values[key] = value;

                options.Template(output, context);

                _values.Remove(key);
            };

        public HandlebarsHelper ClearHelper =>
            (TextWriter output, object context, object[] arguments) =>
            {
                EnsureArgumentsCount(arguments, 1);

                var key = GetArgumentStringValue(arguments, 0) ?? throw new CodeGenHelperException($"Cannot set a value with no key");
                if (_values.ContainsKey(key))
                {
                    _values.Remove(key);
                }
            };

        public HandlebarsHelper GetHelper =>
            (TextWriter output, object context, object[] arguments) =>
            {
                EnsureArgumentsCount(arguments, 1);

                var key = GetArgumentStringValue(arguments, 0) ?? throw new CodeGenHelperException($"Cannot get a value with no key");

                if (_values.TryGetValue(key, out var value))
                {
                    output.Write(value);
                }
            };

        public HandlebarsBlockHelper WithGetHelper =>
            (TextWriter output, HelperOptions options, object context, object[] arguments) =>
            {
                EnsureArgumentsCount(arguments, 1);

                var key = GetArgumentStringValue(arguments, 0) ?? throw new CodeGenHelperException($"Cannot get a value with no key");

                if (_values.TryGetValue(key, out var value))
                {
                    options.Template(output, value);
                }
                else
                {
                    options.Inverse(output, context);
                }
            };

        public override void Setup(HandlebarsConfiguration configuration)
        {
            configuration.Helpers.Add("set", SetHelper);
            configuration.Helpers.Add("get", GetHelper);
            configuration.Helpers.Add("clear", ClearHelper);
            configuration.BlockHelpers.Add("with_get", WithGetHelper);
            configuration.BlockHelpers.Add("with_set", WithSetHelper);
        }
    }
}
