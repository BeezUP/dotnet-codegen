using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodegenUP.CustomHandlebars
{
    public interface IHelperBase
    {
        void Setup(HandlebarsConfiguration configuration);
    }

    public interface IHelper : IHelperBase
    {
    }
}
