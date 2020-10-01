using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace CodegenUP.Tests
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

    public sealed class IgnoreOnLinuxTheory : TheoryAttribute
    {
        public IgnoreOnLinuxTheory()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Skip = "Ignore on Linux";
            }
        }
    }
}
