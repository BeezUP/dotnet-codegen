//using Dotnet.CodeGen.CustomHandlebars.Helpers;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Dynamic;
//using System.Linq;

//namespace Dotnet.CodeGen
//{
//    public class JsonDynamicObject : DynamicObject
//    {
//        private readonly JToken jToken;

//        public JsonDynamicObject(JToken jToken)
//        {
//            this.jToken = jToken;
//        }

//        public override string ToString() => jToken.ToString();

//        public override IEnumerable<string> GetDynamicMemberNames()
//        {
//            //Newtonsoft.Json.Linq.JContainer;
//            //Newtonsoft.Json.Linq.JObject;
//            //Newtonsoft.Json.Linq.JProperty;
//            //Newtonsoft.Json.Linq.JArray;
//            //Newtonsoft.Json.Linq.JValue;
//            //Newtonsoft.Json.Linq.JValue;

//            //jToken.Type == JTokenType

//            return jToken switch
//            {
//                JObject obj => obj.Properties().Select(p => p.Name),
//                //JContainer container => 
//                //_ => new[] { "" },
//                _ => throw new NotImplementedException("GetDynamicMemberNames"),
//            };
//        }

//        static readonly object NULL = new object();

//        public override bool TryGetMember(GetMemberBinder binder, out object result)
//        {
//            var memberName = binder.Name;

//            result = NULL;

//            switch (jToken)
//            {
//                case JObject obj:
//                    var found = obj.TryGetValue(memberName, out var token);
//                    if (found)
//                    {
//#nullable disable
//                        result = new JsonDynamicObject(token);
//#nullable restore
//                        return true;
//                    }
//                    break;
//                    //JContainer container => 
//                    //_ => new[] { "" },
//                    //_ => throw new NotImplementedException("GetDynamicMemberNames"),
//            };

//            return false;
//        }


//        //public enum JTokenType
//        //{
//        //    None = 0,
//        //    Object = 1,
//        //    Array = 2,
//        //    Constructor = 3,
//        //    Property = 4,
//        //    Comment = 5,
//        //    Integer = 6,
//        //    Float = 7,
//        //    String = 8,
//        //    Boolean = 9,
//        //    Null = 10,
//        //    Undefined = 11,
//        //    Date = 12,
//        //    Raw = 13,
//        //    Bytes = 14,
//        //    Guid = 15,
//        //    Uri = 16,
//        //    TimeSpan = 17
//        //}
//    }
//}
