using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace NA.Framework.Core.Extensions
{
    public static class HttpContextExtension
    {
        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRemoteIP(this HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string ip = null;
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ip = context.Request.Headers["X-Forwarded-For"].ToString().Split(',').Where(e => !e.IsNullOrWhiteSpace()).FirstOrDefault();
            }
            if (ip.IsNullOrWhiteSpace())
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }

            return ip;
        }

        /// <summary>
        /// 获取当前服务器域名
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetLocalDomain(this HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            bool succeed = context.Request.Headers.TryGetValue("Host",out var tempValue);
            string domain = succeed ? tempValue.ToString() : null;
            if (domain.IsNullOrWhiteSpace())
            {
                string host = context.Request.Host.Host;
                int? port = context.Request.Host.Port;
                string portStr = port.HasValue ? (":" + port) : null;
                domain = $"{host}{portStr}";
            }

            string scheme = context.Request.Scheme;
            string result = $"{scheme}://{domain}";
            return result;
        }
    }
}
