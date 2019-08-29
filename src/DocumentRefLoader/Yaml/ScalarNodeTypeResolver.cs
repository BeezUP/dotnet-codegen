using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace DocumentRefLoader.Yaml
{
    public class ScalarNodeTypeResolver : ITypeResolver, INodeTypeResolver
    {
        public Type Resolve(Type staticType, object actualValue)
        {
            return staticType;
        }

        public bool Resolve(NodeEvent nodeEvent, ref Type currentType)
        {
            if (currentType == typeof(object))
            {
                if (nodeEvent is Scalar scalar)
                {
                    // Expressions taken from https://github.com/aaubry/YamlDotNet/blob/feat-schemas/YamlDotNet/Core/Schemas/JsonSchema.cs

                    //if (Regex.IsMatch(scalar.Value, @"^(true|false)$", RegexOptions.IgnorePatternWhitespace)
                    //{
                    //    currentType = typeof(bool);
                    //    return true;
                    //}

                    //if (Regex.IsMatch(scalar.Value, @"^-? ( 0 | [1-9] [0-9]* )$", RegexOptions.IgnorePatternWhitespace)
                    //{
                    //    currentType = typeof(int);
                    //    return true;
                    //}

                    //if (Regex.IsMatch(scalar.Value, @"^-? ( 0 | [1-9] [0-9]* ) ( \. [0-9]* )? ( [eE] [-+]? [0-9]+ )?$", RegexOptions.IgnorePatternWhitespace)
                    //{
                    //    currentType = typeof(float);
                    //    return true;
                    //}

                    // Add more cases here if needed
                }
            }
            return false;
        }
    }
}
