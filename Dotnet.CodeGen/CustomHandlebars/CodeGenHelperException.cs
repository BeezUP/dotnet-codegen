using System;
using System.Collections.Generic;
using System.Text;

namespace Dotnet.CodeGen.CustomHandlebars
{
    [Serializable]
    public class CodeGenHelperException : ArgumentException
    {
        public CodeGenHelperException() { }
        public CodeGenHelperException(string message) : base(message) { }
        public CodeGenHelperException(string message, Exception inner) : base(message, inner) { }
        protected CodeGenHelperException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
