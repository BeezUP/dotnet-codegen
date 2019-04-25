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

    public abstract class StandardHelperBase : IStandardHelper
    {
        public StandardHelperBase(string name, Func<HandlebarsHelper> getHelper)
        {
            Name = name;
            _getHelper = getHelper;
        }

        public string Name { get; }

        private readonly Func<HandlebarsHelper> _getHelper;

        public HandlebarsHelper Helper => _getHelper();
    }
}
