using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodegenUP.CustomHandlebars
{
    public interface IHelper
    {
        void Setup(HandlebarsConfiguration configuration);
    }
}
