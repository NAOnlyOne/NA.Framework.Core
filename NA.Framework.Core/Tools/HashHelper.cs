using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public class HashHelper
    {
        /// <summary>
        /// MD5加密（32位）
        /// </summary>
        /// <param name="rawStr"></param>
        /// <returns></returns>
        public static string GetMD5_32(string rawStr)
        {
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(rawStr));
            StringBuilder strBuilder = new StringBuilder();
            foreach (var item in bytes)
            {
                strBuilder.Append(item.ToString("x2"));
            }
            string encodeStr = strBuilder.ToString();
            return encodeStr;
        }

        /// <summary>
        /// MD5加密（16位）
        /// </summary>
        /// <param name="rawStr"></param>
        /// <returns></returns>
        public static string GetMD5_16(string rawStr)
        {
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(rawStr));
            string encodeStr = BitConverter.ToString(bytes,4,8);
            encodeStr = encodeStr.Replace("-","").ToLower();
            return encodeStr;
        }

        public static string GetObjectMD5_32(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            string result = GetMD5_32(json);
            return result;
        }

        public static string GetObjectMD5_16(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            string result = GetObjectMD5_16(json);
            return result;
        }
    }
}
