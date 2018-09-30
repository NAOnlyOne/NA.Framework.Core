using System;
using System.Collections.Generic;
using System.Text;

namespace NA.Framework.Core.Extensions
{
    public static class EnumExtension
    {
        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }
    }
}
