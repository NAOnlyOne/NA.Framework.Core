using System;
using System.Collections.Generic;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public class TimeHelper
    {
        /// <summary>
        /// 时间戳转换为DateTime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime TimeStamp2DateTime(string timeStamp)
        {
            DateTime startTime = new DateTime(1970, 1, 1).ToLocalTime();
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return startTime.Add(toNow);
        }

        /// <summary>
        /// DateTime转换为时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long DateTime2TimeStamp(DateTime time)
        {
            var timeSpan = time.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(timeSpan.TotalMilliseconds);
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetNowTimeStamp()
        {
            return DateTime2TimeStamp(DateTime.UtcNow);
        }

        /// <summary>
        /// 把10位Unix时间戳转换为DateTime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStamp2DateTime(string timeStamp)
        {
            DateTime startTime = new DateTime(1970, 1, 1).ToLocalTime();
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return startTime.Add(toNow);
        }

        /// <summary>
        /// 把13位Unix时间戳转换为DateTime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime MilliTimeStamp2DateTime(string timeStamp)
        {
            DateTime startTime = new DateTime(1970, 1, 1).ToLocalTime();
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return startTime.Add(toNow);
        }

        /// <summary>
        /// DateTime转换为Unix时间戳（10位）
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTime2UnixTimeStamp(DateTime dateTime)
        {
            var timeSpan = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1);
            return Convert.ToInt64(timeSpan.TotalSeconds);
        }

        /// <summary>
        /// 获取当前Unix时间戳（10位）
        /// </summary>
        /// <returns></returns>
        public static long GetNowUnixTimeStamp()
        {
            return DateTime2UnixTimeStamp(DateTime.UtcNow);
        }
    }
}
