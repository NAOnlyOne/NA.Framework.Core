using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace NA.Framework.Core.Extensions
{
    public static class ObjectExtension
    {
        private readonly static JsonSerializerSettings _defaultSetting = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, DateFormatString = "yyyy-MM-dd HH:mm:ss" };

        public static T DeepClone<T>(this T src)
        {
            string json = JsonConvert.SerializeObject(src);
            T des = JsonConvert.DeserializeObject<T>(json);
            return des;
        }

        public static string ToJson(this object obj, JsonSerializerSettings setting = null)
        {
            if (obj == null)
                return null;

            var result = JsonConvert.SerializeObject(obj, setting ?? _defaultSetting);
            return result;
        }
    }
}
