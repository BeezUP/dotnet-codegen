using System;
using System.Collections.Generic;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars
{
#if DEBUG
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class HandlebarsHelperSpecificationAttribute : Attribute
    {
        public HandlebarsHelperSpecificationAttribute(string json, string template, string expectedOutput)
        {
            Json = json;
            Template = template;
            ExpectedOutput = expectedOutput;
        }

        public string Json { get; }
        public string Template { get; }
        public string ExpectedOutput { get; }
    }
#endif
}
