using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace NA.Framework.Core.Tools
{
    public class TypeHelper
    {
        /// <summary>
        /// 把对象转换为目标类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object ConvertType(object obj, Type targetType)
        {
            if (targetType == null)
                return obj;

            if (obj == null || obj == DBNull.Value)
                return targetType.GetTypeInfo().IsValueType ? Activator.CreateInstance(targetType) : null;

            Type underlyingType = Nullable.GetUnderlyingType(targetType);
            //待转换对象的类型与目标类型兼容，则无需转换
            if (targetType.IsAssignableFrom(obj.GetType()))
            {
                return obj;
            }
            //待转换对象的基类为枚举
            else if ((underlyingType ?? targetType).GetTypeInfo().IsEnum)
            {
                //目标类型为可空枚举，且待转换对象为空
                if (underlyingType != null && string.IsNullOrEmpty(obj.ToString()))
                {
                    return null;
                }
                else
                {
                    return Enum.Parse(underlyingType ?? targetType, obj.ToString());
                }
            }
            //目标类型实现了IConvertible，则直接转换
            else if (typeof(IConvertible).IsAssignableFrom(underlyingType ?? targetType))
            {
                try
                {
                    return Convert.ChangeType(obj, underlyingType ?? targetType, null);
                }
                catch
                {
                    return underlyingType == null ? Activator.CreateInstance(targetType) : null
                        ;
                }
            }
            else
            {
                TypeConverter converter = TypeDescriptor.GetConverter(targetType);
                if (converter.CanConvertFrom(obj.GetType()))
                {
                    return converter.ConvertFrom(obj);
                }
                ConstructorInfo constructor = targetType.GetConstructor(Type.EmptyTypes);
                if (constructor != null)
                {
                    object o = constructor.Invoke(null);
                    PropertyInfo[] properties = targetType.GetProperties();
                    Type oldType = obj.GetType();
                    foreach (var property in properties)
                    {
                        PropertyInfo p = oldType.GetProperty(property.Name);
                        if (property.CanWrite && p != null && p.CanRead)
                        {
                            property.SetValue(o, ConvertType(p.GetValue(obj, null), property.PropertyType), null);
                        }
                    }
                    return o;
                }
            }
            return obj;
        }

        /// <summary>
        /// 获取目标类型的默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefaultValue(Type type)
        {
            var obj = type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
            return obj;
        }

        /// <summary>
        /// 判断对象是否为所属类型默认值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDefaultValue(object value)
        {
            if (value == null)
                return true;
            else
            {
                bool isDefault = false;
                Type type = value.GetType();
                if (type.GetTypeInfo().IsValueType)
                {
                    var defaultValue = GetDefaultValue(type);
                    isDefault = value.Equals(defaultValue);
                }
                return isDefault;
            }
        }

        /// <summary>
        /// 获取指定对象的属性->值集合
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propFlags">属性标志</param>
        /// <param name="includeDefaultValue">是否包含默认值</param>
        /// <returns></returns>
        public static Dictionary<string, object> GetPropertyValues(object obj, BindingFlags propFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, bool includeDefaultValue = true)
        {
            if (obj == null)
                return null;

            Type type = obj.GetType();
            var result = new Dictionary<string, object>();
            foreach (var property in type.GetProperties(propFlags))
            {
                bool canInsert = false;
                var value = property.GetValue(obj);
                canInsert = IsDefaultValue(value) == false || includeDefaultValue;

                if (canInsert)
                {
                    result.Add(property.Name, value);
                }
            }
            return result;
        }
    }
}
