using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public class ImageHelper
    {
        /// <summary>
        /// 获取图片的base64编码
        /// </summary>
        /// <param name="imgPath"></param>
        /// <returns></returns>
        public static string ConvertImg2Base64(string imgPath)
        {
            if (imgPath == null)
                throw new ArgumentNullException(nameof(imgPath));

            if (!File.Exists(imgPath))
                throw new FileNotFoundException("指定图片不存在", imgPath);

            byte[] bytes = File.ReadAllBytes(imgPath);
            string base64Str = Convert.ToBase64String(bytes);
            return base64Str;
        }
    }
}
