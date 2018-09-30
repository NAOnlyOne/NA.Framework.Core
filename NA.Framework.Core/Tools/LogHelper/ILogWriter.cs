using System;
using System.Collections.Generic;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public interface ILogWriter
    {
        void WriteLog(string message, ELogLevel logLevel);
    }
}
