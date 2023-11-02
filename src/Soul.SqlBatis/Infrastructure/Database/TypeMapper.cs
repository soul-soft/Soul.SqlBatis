using System;
using System.Data;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public static class TypeMapper
    {
        public static MethodInfo IsDBNullMethod = typeof(IDataRecord).GetMethod(nameof(IDataRecord.IsDBNull), new Type[] { typeof(int) });
        
        public static MethodInfo FindMethod(Type type)
        {
            if (type == typeof(short))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt16), new Type[] { typeof(int) });
            }
            if (type == typeof(int))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt32), new Type[] { typeof(int) });
            }
            if (type == typeof(long))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt64), new Type[] { typeof(int) });
            }
            if (type == typeof(float))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetFloat), new Type[] { typeof(int) });
            }
            if (type == typeof(double))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDouble), new Type[] { typeof(int) });
            }
            if (type == typeof(string))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetString), new Type[] { typeof(int) });
            }
            if (type == typeof(DateTime))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDateTime), new Type[] { typeof(int) });
            }
            if (type == typeof(Guid))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetGuid), new Type[] { typeof(int) });
            }
            if (type == typeof(bool))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetBoolean), new Type[] { typeof(int) });
            }
            if (type == typeof(byte))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetByte), new Type[] { typeof(int) });
            }
            if (type == typeof(byte[]))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetBytes), new Type[] { typeof(int), typeof(long), typeof(byte[]), typeof(int), typeof(int) });
            }
            if (type == typeof(char))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetChar), new Type[] { typeof(int) });
            }
            if (type == typeof(char[]))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetChars), new Type[] { typeof(int), typeof(long), typeof(byte[]), typeof(int), typeof(int) });
            }
            return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetValue), new Type[] { typeof(int) });
        }
    }
}
