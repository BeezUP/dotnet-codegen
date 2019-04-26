using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars
{
    public interface IStandardHelper
    {
        string Name { get; }

        HandlebarsHelper Helper { get; }
    }

    public abstract class StandardHelperBase : HelperBase, IStandardHelper
    {
        public StandardHelperBase(string name) : base(name) { }
        public abstract HandlebarsHelper Helper { get; }
    }
}
