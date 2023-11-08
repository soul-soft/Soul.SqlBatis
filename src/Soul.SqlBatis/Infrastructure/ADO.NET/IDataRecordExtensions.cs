using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    internal static class IDataRecordExtensions
	{
        private readonly static ConcurrentDictionary<Type, Delegate> _delegates = new ConcurrentDictionary<Type, Delegate>();

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
			var buffer = new byte[0];
			var length = dr.GetBytes(i, 0, buffer, 0, buffer.Length);
			return buffer.Take((int)length).ToArray();
		}
	
		public static char[] GetChars(this IDataRecord dr, int i)
		{
			var buffer = new char[0];
			var length = dr.GetChars(i, 0, buffer, 0, buffer.Length);
			return buffer.Take((int)length).ToArray();
		}

        public static Delegate GetDelegate(Type type)
        {
            return _delegates.GetOrAdd(type, key =>
            {
                var methods = typeof(IDataRecord).GetMethods();
                if (type == typeof(byte))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetByte))
                    .First()
                    .CreateDelegate(typeof(Func<int, byte>));
                }
                if (type == typeof(char))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetChar))
                    .First()
                    .CreateDelegate(typeof(Func<int, char>));
                }
                if (type == typeof(short))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetInt16))
                    .First()
                    .CreateDelegate(typeof(Func<int, short>));
                }
                if (type == typeof(int))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetInt32))
                    .First()
                    .CreateDelegate(typeof(Func<int, int>));
                }
                if (type == typeof(long))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetInt64))
                    .First()
                    .CreateDelegate(typeof(Func<int, long>));
                }
                if (type == typeof(float))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetFloat))
                    .First()
                    .CreateDelegate(typeof(Func<int, float>));
                }
                if (type == typeof(double))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetDouble))
                    .First()
                    .CreateDelegate(typeof(Func<int, double>));
                }
                if (type == typeof(decimal))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetDecimal))
                    .First()
                    .CreateDelegate(typeof(Func<int, decimal>));
                }
                if (type == typeof(bool))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetBoolean))
                    .First()
                    .CreateDelegate(typeof(Func<int, bool>));
                }
                if (type == typeof(Guid))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetGuid))
                    .First()
                    .CreateDelegate(typeof(Func<int, Guid>));
                }
                if (type == typeof(DateTime))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetDateTime))
                    .First()
                    .CreateDelegate(typeof(Func<int, DateTime>));
                }
                if (type == typeof(string))
                {
                    var method = methods.Where(a => a.Name == nameof(IDataRecord.GetString)).First();
                    return method.CreateDelegate(typeof(Func<int, string>));
                }
                if (type == typeof(byte[]))
                {
                    return typeof(IDataRecordExtensions).GetMethods()
                    .Where(a => a.Name == nameof(IDataRecordExtensions.GetBytes))
                    .First()
                    .CreateDelegate(typeof(Func<int, byte[]>));
                }
                if (type == typeof(char[]))
                {
                    return typeof(IDataRecordExtensions).GetMethods()
                    .Where(a => a.Name == nameof(IDataRecordExtensions.GetChars))
                    .First()
                    .CreateDelegate(typeof(Func<int, byte[]>));
                }
                return methods.Where(a => a.Name == nameof(IDataRecord.GetValue))
                    .First()
                    .CreateDelegate(typeof(Func<int, object>));
            });
        }
    }
}
