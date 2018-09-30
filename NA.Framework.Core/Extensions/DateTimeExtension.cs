using System;
using System.Collections.Generic;
using System.Text;

namespace NA.Framework.Core.Extensions
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 获取中国时区时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime ToCNZone(this DateTime time)
        {
            var timeSpan = new TimeSpan(8, 0, 0) - TimeZoneInfo.Local.BaseUtcOffset;
            var cnTime = time.Add(timeSpan);
            return cnTime;
        }

        public static string ToString(this DateTime? time, string format)
        {
            if (time == null)
                return null;
            else
                return time.Value.ToString(format);
        }
    }
}
