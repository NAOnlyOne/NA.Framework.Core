using System;
using System.Collections.Generic;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public abstract class StackBaseLogger : System.IO.TextWriter
    {
        public abstract override Encoding Encoding { get; }

        public abstract override void WriteLine(string format, object arg0);

        public abstract override void WriteLine(string format, object arg0, object arg1);

        public abstract override void WriteLine(string format, object arg0, object arg1, object arg2);

        public abstract override void WriteLine(string format, params object[] arg);

        public abstract override void WriteLine(string value);

        public abstract void Log(string text);
    }
}
