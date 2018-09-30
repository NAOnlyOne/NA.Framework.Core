using NA.Framework.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public class TextWriter : ILogWriter
    {
        /// <summary>
        /// 日志存储物理路径
        /// </summary>
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("日志存储路径不能为空");
                }
                else
                {
                    _path = value;
                }
            }
        }
        private string _path;

        public TextWriter(string path)
        {
            Path = path;
        }

        public void WriteLog(string message, ELogLevel logLevel)
        {
            string fileName = System.IO.Path.Combine(_path,$"Logs-{DateTime.Now.ToCNZone().ToString("yyyy-MM-dd")}");
            using (var sw = File.AppendText(fileName))
            {
                string startStr = $"[Start Level:{logLevel.ToString()} Time:{DateTime.Now.ToCNZone().ToString("yyyy-MM-dd HH:mm:ss,fff")}]";
                sw.WriteLine(startStr);
                sw.WriteLine(message);
                string endStr = "[End]";
                sw.WriteLine(endStr);
                sw.WriteLine();
                sw.Flush();
            }
        }
    }
}
