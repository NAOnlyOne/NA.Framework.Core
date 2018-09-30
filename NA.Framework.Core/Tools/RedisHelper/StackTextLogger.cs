using System;
using System.Collections.Generic;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public class StackTextLogger : StackBaseLogger
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void Log(string text)
        {
            LogHelper.WriteLog(ELogType.Redis, new LogInfo(text, ELogLevel.Info));
        }

        public override void WriteLine(string format, object arg0)
        {
            string info = string.Format(format,arg0);
            LogHelper.WriteLog(ELogType.Redis,new LogInfo(info,ELogLevel.Info));
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            string info = string.Format(format, arg0, arg1);
            LogHelper.WriteLog(ELogType.Redis, new LogInfo(info, ELogLevel.Info));
        }

        public override void WriteLine(string format, object arg0, object arg1,object arg2)
        {
            string info = string.Format(format, arg0, arg1, arg2);
            LogHelper.WriteLog(ELogType.Redis, new LogInfo(info, ELogLevel.Info));
        }

        public override void WriteLine(string format,params object[] arg)
        {
            string info = string.Format(format,arg);
            LogHelper.WriteLog(ELogType.Redis, new LogInfo(info, ELogLevel.Info));
        }

        public override void WriteLine(string value)
        {
            LogHelper.WriteLog(ELogType.Redis, new LogInfo(value, ELogLevel.Info));
        }
    }
}
