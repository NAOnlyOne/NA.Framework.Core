using System;
using System.Collections.Generic;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public class APIInfo
    {
        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 调用参数
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// 返回结果
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public EAPIMethod Method { get; set; }

        public APIInfo(string url, object parameter, object result, EAPIMethod method)
        {
            Url = url;
            Parameter = parameter;
            Result = result;
            Method = method;
        }
    }
}
