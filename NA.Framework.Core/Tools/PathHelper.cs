using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public class PathHelper
    {
        private readonly static char _separator = Path.DirectorySeparatorChar;

        public static string ContentRootPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_contentRootPath))
                {
                    throw new Exception("程序根目录不能为空，请调用Init函数初始化工具类");
                }
                return _contentRootPath;
            }
        }
        private static string _contentRootPath;

        public static void Init(string contentRootPath)
        {
            if (string.IsNullOrWhiteSpace(contentRootPath))
            {
                throw new Exception("程序根目录不能为空");
            }

            _contentRootPath = contentRootPath;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool CreateDirectory(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
                return false;

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 获取绝对路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string MapPath(string filePath)
        {
            if (filePath == null)
                return null;

            string absolutePath = Path.IsPathRooted(filePath) || IsAbsolute(filePath) ? filePath : Path.Combine(ContentRootPath, filePath.TrimStart('~', '/')).Replace('/', _separator);
            return absolutePath;
        }

        public static string MapPath(string path, string fileName)
        {
            string filePath = Path.Combine(path, fileName);
            return MapPath(filePath);
        }

        private static bool IsAbsolute(string path)
        {
            bool result = Path.VolumeSeparatorChar == ':' ? path.IndexOf(Path.VolumeSeparatorChar) > 0 : path.IndexOf('\\') > 0;
            return result;
        }
    }
}
