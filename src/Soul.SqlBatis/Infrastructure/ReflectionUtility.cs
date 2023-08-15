using System;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    internal static class ReflectionUtility
    {
        /// <summary>
        /// 是否是Nullable类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }
        /// <summary>
        /// 获取非Nullable类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetUnderlyingType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }      
    }
}
