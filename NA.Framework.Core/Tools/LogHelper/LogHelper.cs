using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace NA.Framework.Core.Tools
{
    public class LogHelper
    {
        private static Dictionary<string, Queue<LogInfo>> _queueDict = new Dictionary<string, Queue<LogInfo>>();
        private static bool _hadInit = false;

        /// <summary>
        /// 初始化工具类
        /// </summary>
        /// <param name="logSavePath"></param>
        public static void Init(string logSavePath)
        {
            if (string.IsNullOrWhiteSpace(logSavePath))
            {
                throw new Exception("日志记录路径不能为空");
            }

            //根据TextType的日志类型注册日志记录器
            foreach (var type in Enum.GetNames(typeof(ELogType)))
            {
                string path = PathHelper.MapPath(Path.Combine(logSavePath, type));
                PathHelper.CreateDirectory(path);
                RegisterWriter(new TextWriter(path), HashHelper.GetMD5_16(type));
            }

            _hadInit = true;
        }

        /// <summary>
        /// 注册日志记录器
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static bool RegisterWriter(ILogWriter writer, string identifier)
        {
            if (writer == null || string.IsNullOrWhiteSpace(identifier))
                return false;

            identifier = identifier.ToLower();
            if (_queueDict.ContainsKey(identifier))
                return false;

            Queue<LogInfo> infoQueue = new Queue<LogInfo>();
            _queueDict.Add(identifier, infoQueue);
            ThreadPool.QueueUserWorkItem(i =>
            {
                while (true)
                {
                    if (infoQueue.Count > 0)
                    {
                        lock (infoQueue)
                        {
                            LogInfo logInfo = infoQueue.Dequeue();
                            writer.WriteLog(logInfo.Message, logInfo.LogLevel);
                        }
                    }
                    else
                        Thread.Sleep(100);
                }
            });
            return true;
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        public static bool WriteLog(string identifier, LogInfo logInfo)
        {
            if (string.IsNullOrWhiteSpace(identifier) || logInfo == null || !_queueDict.ContainsKey(identifier.ToLower()))
                return false;

            var queue = _queueDict[identifier.ToLower()];
            lock (queue)
            {
                queue.Enqueue(logInfo);
            }
            return true;
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public static bool WriteLog(string identifier, string message, ELogLevel logLevel = ELogLevel.Info)
        {
            LogInfo logInfo = new LogInfo(message, logLevel);
            return WriteLog(identifier, logInfo);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="type"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool WriteLog(ELogType type, LogInfo info)
        {
            if (!_hadInit)
            {
                throw new Exception("请调用Init函数初始化本工具类");
            }

            string identifier = HashHelper.GetMD5_16(type.ToString());
            return WriteLog(identifier, info);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public static bool WriteLog(ELogType type, string message, ELogLevel logLevel = ELogLevel.Info)
        {
            if (!_hadInit)
            {
                throw new Exception("请调用Init函数初始化本工具类");
            }

            string identifier = HashHelper.GetMD5_16(type.ToString());
            return WriteLog(identifier, new LogInfo(message, logLevel));
        }

        /// <summary>
        /// 记录API调用情况
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool WriteAPILog(APIInfo info,string additionalMsg = "")
        {
            if (!_hadInit)
            {
                throw new Exception("请调用Init函数初始化本工具类");
            }

            if (info == null || string.IsNullOrWhiteSpace(info.Url) || info.Result == null)
                return false;

            //URL
            string urlStr = $"接口：{info.Method.ToString()} {info.Url}";

            //参数
            string paramStr = "\r\n参数：";
            if (info.Parameter != null)
            {
                var paramPropValues = TypeHelper.GetPropertyValues(info.Parameter, includeDefaultValue: false);
                foreach (var kv in paramPropValues)
                {
                    paramStr += $"\r\n{kv.Key} : {kv.Value}";
                }
            }

            //结果
            string resultStr = "\r\n结果：";
            var resultPropValues = TypeHelper.GetPropertyValues(info.Result, includeDefaultValue: false);
            foreach (var kv in resultPropValues)
            {
                resultStr += $"\r\n{kv.Key} : {kv.Value}";
            }

            //附加信息
            if (!string.IsNullOrWhiteSpace(additionalMsg))
            {
                additionalMsg = $"\r\n附加信息：{additionalMsg}";
            }

            string message = urlStr + paramStr + resultStr + additionalMsg;
            LogInfo logInfo = new LogInfo(message, ELogLevel.Info);
            string identifier = HashHelper.GetMD5_16(ELogType.API.ToString());
            return WriteLog(identifier, logInfo);
        }

        /// <summary>
        /// 记录异常
        /// </summary>
        /// <param name="ep"></param>
        /// <param name="additionalMsg"></param>
        /// <returns></returns>
        public static bool WriteExceptionLog(Exception ep, string additionalMsg = "")
        {
            if (!_hadInit)
            {
                throw new Exception("请调用Init函数初始化本工具类");
            }

            if (ep == null)
                return false;

            //编码
            long id = TimeHelper.GetNowTimeStamp();
            string idStr = $"ID：{id.ToString()}\r\n";

            int indexOfIn = ep.StackTrace != null ? ep.StackTrace.ToLower().IndexOf(" in ") : -1;
            string positionStr = null;
            if (indexOfIn >= 0)
                positionStr = "\r\nError happens" + ep.StackTrace.Substring(indexOfIn);

            //附加信息
            if (!string.IsNullOrWhiteSpace(additionalMsg))
            {
                additionalMsg = $"\r\n附加信息：{additionalMsg}";
            }

            string errorMsg = $"{idStr}{ep.Message}{positionStr}{additionalMsg}";

            LogInfo logInfo = new LogInfo(errorMsg,ELogLevel.Error);
            string identitifer = HashHelper.GetMD5_16(ELogType.Exception.ToString());
            bool succeed = WriteLog(identitifer,logInfo);
            if (succeed)
            {
                WriteExceptionLog(ep.InnerException,$"{id} 的内部异常");
            }
            return succeed;
        }
    }
}
