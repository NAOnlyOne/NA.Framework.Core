using NA.Framework.Core.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NA.Framework.Core.Tools
{
    /// <summary>
    /// 共有方法
    /// </summary>
    public partial class APIHelper
    {
        private static readonly HttpClient _client = new HttpClient { Timeout = new TimeSpan(0, 0, 30) };
        private static readonly HttpMethod[] _noneBodyMethods = new HttpMethod[] { HttpMethod.Get, HttpMethod.Head, HttpMethod.Trace };

        /// <summary>
        /// 调用远程接口
        /// </summary>
        /// <param name="api">接口地址</param>
        /// <param name="method">Http方法</param>
        /// <param name="parameters">传入的参数，如：new { ID=1 , Name="Tester" }</param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<byte[]> InvokeAPIAsync(string api, HttpMethod method, object parameters = null, object content = null)
        {
            if (api.IsNullOrWhiteSpace())
                throw new ArgumentException("接口地址不能为空", nameof(api));
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            string paramStr = string.Empty;
            if (parameters != null)
            {
                paramStr = GetQueryStr(parameters);
            }

            string contentStr = string.Empty;
            if (content != null)
            {
                contentStr = GetQueryStr(content);
            }

            var result = await InvokeAsync(api, method, paramStr, contentStr);
            return result;
        }

        public static T GetObj<T>(string api, HttpMethod method, object query = null, object content = null, EEncodeType responseType = EEncodeType.UTF8)
        {
            var result = GetObjAsync<T>(api, method, query, content, responseType).Result;
            return result;
        }

        /// <summary>
        /// 调用API，获取Json结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api"></param>
        /// <param name="parameters"></param>
        /// <param name="method"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static async Task<T> GetObjAsync<T>(string api, HttpMethod method, object query = null, object content = null, EEncodeType responseType = EEncodeType.UTF8)
        {
            var bytes = await InvokeAPIAsync(api, method, query, content);
            string responseStr = Encoding.GetEncoding(GetCodeName(responseType)).GetString(bytes);
            T result = JsonConvert.DeserializeObject<T>(responseStr);
            return result;
        }

        /// <summary>
        /// 调用API，获取字符串结果
        /// </summary>
        /// <param name="api"></param>
        /// <param name="method"></param>
        /// <param name="query"></param>
        /// <param name="content"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static string GetString(string api, HttpMethod method, object query = null, object content = null, EEncodeType responseType = EEncodeType.UTF8)
        {
            var result = GetStringAsync(api, method, query, content, responseType).Result;
            return result;
        }

        /// <summary>
        /// 调用API，获取字符串结果
        /// </summary>
        /// <param name="api"></param>
        /// <param name="method"></param>
        /// <param name="query"></param>
        /// <param name="content"></param>
        /// <param name="responseType"></param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(string api, HttpMethod method, object query = null, object content = null, EEncodeType responseType = EEncodeType.UTF8)
        {
            var bytes = await InvokeAPIAsync(api, method, query, content);
            string responseStr = Encoding.GetEncoding(GetCodeName(responseType)).GetString(bytes);
            return responseStr;
        }
    }

    /// <summary>
    /// 私有方法
    /// </summary>
    public partial class APIHelper
    {
        private static string GetQueryStr(object query)
        {
            Type type = query.GetType();
            if (type.GetTypeInfo().IsValueType)
                throw new ArgumentException("无法解释值类型参数", nameof(query));

            StringBuilder strBuilder = new StringBuilder();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string propName = prop.Name;
                var value = prop.GetValue(query);
                string strValue = value == null ? string.Empty : value.ToString();

                string typeFullName = prop.PropertyType.FullName;
                if (strValue.Equals(typeFullName))
                {
                    throw new NotSupportedException($"发现无法解释的类型：{typeFullName}");
                }

                strBuilder.Append($"{propName.ToUrlEncode()}={strValue.ToUrlEncode()}&");
            }
            return strBuilder.ToString();
        }

        private static async Task<byte[]> InvokeAsync(string api, HttpMethod method, string paramStr = null, string contentStr = null)
        {
            if (api.IsNullOrWhiteSpace())
                throw new ArgumentException("接口地址不能为空", nameof(api));
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            HttpContent httpContent = null;
            if (HasBody(method) && contentStr != null)
            {
                httpContent = new StringContent(contentStr);
                httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            }
            else
            {
                httpContent = new StringContent("");
            }

            if (paramStr.IsNullOrWhiteSpace())
            {
                int index = api.IndexOf('?');
                char connChar = index == -1 ? '?' : '&';
                api = $"{api}{connChar}{paramStr}";
            }

            try
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage
                {
                    Method = method,
                    Content = httpContent,
                    RequestUri = new Uri(api)
                };
                HttpResponseMessage responseMessage = await _client.SendAsync(requestMessage);
                var result = await responseMessage.Content.ReadAsByteArrayAsync();
                return result;
            }
            catch (Exception ep)
            {
                throw new HttpRequestException("API调用失败，详见内部异常", ep);
            }
        }

        private static bool HasBody(HttpMethod method)
        {
            return !_noneBodyMethods.Contains(method);
        }

        private static string GetCodeName(EEncodeType type)
        {
            switch (type)
            {
                case EEncodeType.UTF8:
                    return "utf-8";
                case EEncodeType.UTF16:
                    return "utf-16";
                case EEncodeType.UTF32:
                    return "utf-32";
                case EEncodeType.ASCII:
                    return "us-ascii";
                case EEncodeType.GBK:
                    return "gbk";
                default:
                    return "utf-8";
            }
        }
    }

    public enum EEncodeType
    {
        UTF8,
        UTF16,
        UTF32,
        ASCII,
        GBK
    }
}
