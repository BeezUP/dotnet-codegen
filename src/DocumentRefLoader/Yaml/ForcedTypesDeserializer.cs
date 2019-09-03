using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace DocumentRefLoader.Yaml
{
    class ForcedNodeTypeResolver : INodeTypeResolver
    {
        private readonly INodeTypeResolver _inner;
        private readonly IEnumerable<(Func<string, bool> predicate, Type type)> _typesDetectors;

        public ForcedNodeTypeResolver(INodeTypeResolver inner, IDictionary<Func<string, bool>, Type> overrideTypesDetectors = null)
        {
            _inner = inner;
            _typesDetectors = (
                overrideTypesDetectors ?? new Dictionary<Func<string, bool>, Type>()
                {
                    { s => bool.TryParse(s, out var _), typeof(bool) },
                    { s => int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var _), typeof(int) },
                    { s => float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var _), typeof(float) },
                })
                .Select(kv => (kv.Key, kv.Value))
                .ToArray();
        }

        public bool Resolve(NodeEvent nodeEvent, ref Type currentType)
        {
            switch (nodeEvent)
            {
                case Scalar scalar:
                    if (scalar.Style == ScalarStyle.SingleQuoted || scalar.Style == ScalarStyle.DoubleQuoted)
                        break;

                    foreach (var pat in _typesDetectors)
                    {
                        if (pat.predicate(scalar.Value))
                        {
                            currentType = pat.type;
                            return true;
                        }
                    }
                    break;
            }

            return _inner.Resolve(nodeEvent, ref currentType);
        }
    }
}
