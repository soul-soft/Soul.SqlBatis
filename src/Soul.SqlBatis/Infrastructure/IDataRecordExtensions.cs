using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	internal static class IDataRecordExtensions
	{
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
	}

	internal class DataRecordField
	{
		public Type Type { get; }
		public string Name { get; }
		public int Ordinal { get; }
		public DataRecordField(Type type, string name, int ordinal)
		{
			Type = type;
			Name = name;
			Ordinal = ordinal;
		}
	}
}
