using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars
{
    public interface IBlockHelper
    {
        string Name { get; }

        HandlebarsBlockHelper Helper { get; }
    }

    public abstract class BlockHelperBase : IBlockHelper
    {
        public BlockHelperBase(string name, Func<HandlebarsBlockHelper> getHelper)
        {
            Name = name;
            _getHelper = getHelper;
        }

        public string Name { get; }

        private readonly Func<HandlebarsBlockHelper> _getHelper;
        public HandlebarsBlockHelper Helper => _getHelper();
    }
}
