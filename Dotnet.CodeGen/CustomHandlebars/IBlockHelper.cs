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

    public abstract class BlockHelperBase : HelperBase, IBlockHelper
    {
        public BlockHelperBase(string name) : base(name) { }

        public abstract HandlebarsBlockHelper Helper { get; }
    }
}
