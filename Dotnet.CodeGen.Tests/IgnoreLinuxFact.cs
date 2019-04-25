using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace Dotnet.CodeGen.Tests
{
    public sealed class IgnoreOnLinuxFact : FactAttribute
    {
        public IgnoreOnLinuxFact()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Skip = "Ignore on Linux";
            }
        }
    }
}
