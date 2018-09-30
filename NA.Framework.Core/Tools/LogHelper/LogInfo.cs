using System;
using System.Collections.Generic;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public class LogInfo
    {
        public string Message  { get; set; }

        public ELogLevel LogLevel { get; set; }

        public LogInfo(string message, ELogLevel logLevel)
        {
            Message = message;
            LogLevel = logLevel;
        }
    }
}
