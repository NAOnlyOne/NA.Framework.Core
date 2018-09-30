using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace NA.Framework.Core.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToCapitalize(this string str)
        {
            if (str == null)
                return null;
            else if (str.Length <= 0)
                return string.Empty;
            else
                return str[0].ToString().ToUpper() + str.Substring(1);
        }

        public static string ToBase64String(this string str)
        {
            if (str == null)
                return null;
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        public static int ToBKDRHash(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;

            int seed = 131;
            int hash = 0;

            char[] chars = str.ToCharArray();

            for (int i = 0; i < str.Length; i++)
            {
                hash = hash * seed + str[i];
            }
            return hash & 0x7FFFFFFF;
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string jsonStr)
        {
            if (string.IsNullOrWhiteSpace(jsonStr))
                return default(T);
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        public static int ToInt(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            else
            {
                int.TryParse(str, out int result);
                return result;
            }
        }

        public static int? ToIntNull(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return null;
            else
            {
                bool succeed = int.TryParse(str, out int result);
                if (succeed)
                    return result;
                else
                    return null;
            }
        }

        public static bool ToBool(this string str)
        {
            if (str.IsNullOrWhiteSpace())
                return false;
            else
                return Convert.ToBoolean(str);
        }

        public static ResultType ToEnum<ResultType>(this string str) where ResultType : struct
        {
            if (!typeof(ResultType).IsEnum)
                throw new ArgumentException("类型参数必须为枚举类型", nameof(ResultType));

            if (Enum.TryParse<ResultType>(str, out var result))
                return result;
            else
                return default(ResultType);
        }

        public static DateTime? ToDateTimeNull(this string str)
        {
            if (str.IsNullOrWhiteSpace())
                return null;
            else
            {
                bool succeed = DateTime.TryParse(str, out DateTime result);
                if (succeed)
                    return result;
                else
                    return null;
            }
        }

        public static string ToUrlEncode(this string str)
        {
            if (str == null)
                return null;
            else
                return WebUtility.UrlEncode(str);
        }

        public static string ToUrlDecode(this string str)
        {
            if (str == null)
                return null;
            else
                return WebUtility.UrlDecode(str);
        }

        /// <summary>
        /// 判断是否为身份证号码
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool IsIdCardNo(this string num)
        {
            if (!num.IsNullOrWhiteSpace() && Regex.IsMatch(num, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", RegexOptions.IgnoreCase))
                return true;
            else
                return false;
        }

        public static bool IsUri(this string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.Absolute);
        }

        public static bool IsIP(this string ip)
        {
            return !ip.IsNullOrWhiteSpace() && Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static bool IsSubnetMask(this string subnetMask)
        {
            return !subnetMask.IsNullOrWhiteSpace() && Regex.IsMatch(subnetMask, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)/(\d|[12]\d|3[0-2])$");
        }

        public static bool ContainsChinese(this string str)
        {
            if (str.IsNullOrWhiteSpace())
                return false;

            var regex = new Regex("[\u4e00-\u9fa5]");
            foreach (var ch in str.ToCharArray())
            {
                if (regex.IsMatch(ch.ToString()))
                    return true;
            }

            return false;
        }

        public static bool IsMatchSubnetMask(this string ip, string subnetMask)
        {
            if (!ip.IsIP())
                throw new ArgumentException("IP格式不正确", nameof(ip));
            if (!subnetMask.IsSubnetMask())
                throw new ArgumentException("子网掩码格式不正确", nameof(subnetMask));

            var arr = subnetMask.Split('/');
            string subnet = arr[0];
            string mask = arr[1];
            int maskInt = int.Parse(mask);

            string ipBinStr = ConvertIp2Binary(ip);
            string subnetBinStr = ConvertIp2Binary(subnet);

            bool valid = ipBinStr.Replace(".", "").Substring(0, maskInt) == subnetBinStr.Replace(".", "").Substring(0, maskInt);
            return valid;
        }

        private static string ConvertIp2Binary(string ip)
        {
            string[] segments = ip.Split('.');
            var binarys = segments.Select(seg =>
            {
                int num = int.Parse(seg);

                string binary = Convert.ToString(num, 2).PadLeft(8, '0');
                return binary;
            });

            string result = string.Join('.', binarys);
            return result;
        }
    }
}
