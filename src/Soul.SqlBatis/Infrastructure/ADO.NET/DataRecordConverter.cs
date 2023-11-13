using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    internal static class DataRecordConverter
    {
        private readonly static ConcurrentDictionary<Type, MethodInfo> _converters = new ConcurrentDictionary<Type, MethodInfo>();

        public static List<DataRecordField> GetFields(this IDataRecord dr)
        {
            var list = new List<DataRecordField>();
            for (int i = 0; i < dr.FieldCount; i++)
            {
                var name = dr.GetName(i);
                var type = dr.GetFieldType(i);
                list.Add(new DataRecordField(type, name, i));
            }
            return list;
        }

        public static byte[] GetBytes(this IDataRecord dr, int i)
        {
            var buffer = new byte[SqlMapper.Settings.BinaryBufferSize];
            var length = dr.GetBytes(i, 0, buffer, 0, buffer.Length);
            return buffer.Take((int)length).ToArray();
        }

        public static char[] GetChars(this IDataRecord dr, int i)
        {
            var buffer = new char[SqlMapper.Settings.TextBufferSize];
            var length = dr.GetChars(i, 0, buffer, 0, buffer.Length);
            return buffer.Take((int)length).ToArray();
        }

        public static MethodInfo GetGetMethod(Type type)
        {
            return _converters.GetOrAdd(type, key =>
            {
                var methods = typeof(IDataRecord).GetMethods();
                if (type == typeof(byte))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetByte)).First();
                }
                if (type == typeof(char))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetChar)).First();
                }
                if (type == typeof(short))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetInt16)).First();
                }
                if (type == typeof(int))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetInt32)).First();
                }
                if (type == typeof(long))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetInt64)).First();
                }
                if (type == typeof(float))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetFloat)).First();
                }
                if (type == typeof(double))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetDouble)).First();
                }
                if (type == typeof(decimal))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetDecimal)).First();
                }
                if (type == typeof(bool))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetBoolean)).First();
                }
                if (type == typeof(Guid))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetGuid)).First();
                }
                if (type == typeof(DateTime))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetDateTime)).First();
                }
                if (type == typeof(string))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetString)).First();
                }
                if (type == typeof(byte[]))
                {
                    return typeof(DataRecordConverter).GetMethods().Where(a => a.Name == nameof(GetBytes)).First();
                }
                if (type == typeof(char[]))
                {
                    return typeof(DataRecordConverter).GetMethods().Where(a => a.Name == nameof(GetChars)).First();
                }
                return methods.Where(a => a.Name == nameof(IDataRecord.GetValue)).First();
            });
        }
    }
}
