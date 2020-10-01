using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace CodegenUP
{
    public static class JsonHelper
    {
        public static object GetDynamicObjectFromJson(JToken json)
        {
            //return new JsonDynamicObject(json);

            if (json.Type == JTokenType.Array)
            {
                return JsonConvert.DeserializeObject<ExpandoObject[]>(json.ToString());
            }
            else
            {
                return JsonConvert.DeserializeObject<ExpandoObject>(json.ToString());
            }
        }
    }
}
