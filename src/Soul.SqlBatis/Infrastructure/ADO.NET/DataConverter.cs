using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	internal static class DataConverter
	{
		private readonly static List<MethodInfo> _converters;
		
		public static MethodInfo IsDBNullMethod = typeof(IDataRecord).GetMethod(nameof(IDataRecord.IsDBNull), new Type[] { typeof(int) });

		public static MethodInfo GetConverter(Type type)
		{
			return typeof(DataConverter).GetMethods().Where(a => a.ReturnType == type).FirstOrDefault();
		}

		#region Converters
		private static byte ToByte(this IDataRecord dr, int i)
		{
			return dr.GetByte(i);
		}
		private static byte[] ToBytes(this IDataRecord dr, int i)
		{
			var buffer = new byte[SqlMapper.Settings.BinaryBufferSize];
			var length = dr.GetBytes(i, 0, buffer, 0, buffer.Length);
			return buffer.Take((int)length).ToArray();
		}
		private static char ToChar(this IDataRecord dr, int i)
		{
			return dr.GetChar(i);
		}
		private static char[] ToChars(this IDataRecord dr, int i)
		{
			var buffer = new char[SqlMapper.Settings.TextBufferSize];
			var length = dr.GetChars(i, 0, buffer, 0, buffer.Length);
			return buffer.Take((int)length).ToArray();
		}
		private static bool ToBoolean(this IDataRecord dr, int i)
		{
			return dr.ToBoolean(i);
		}
		private static short ToInt16(this IDataRecord dr, int i)
		{
			return dr.GetInt16(i);
		}
		private static int ToInt32(this IDataRecord dr, int i)
		{
			return dr.GetInt32(i);
		}
		private static long ToInt64(this IDataRecord dr, int i)
		{
			return dr.GetInt64(i);
		}
		private static float GetFloat(this IDataRecord dr, int i)
		{
			return dr.GetFloat(i);
		}
		private static double GetDouble(this IDataRecord dr, int i)
		{
			return dr.GetDouble(i);
		}
		private static decimal GetDecimal(this IDataRecord dr, int i)
		{
			return dr.GetDecimal(i);
		}
		private static Guid GetGuid(this IDataRecord dr, int i)
		{
			return dr.GetGuid(i);
		}
		private static DateTime GetDateTime(this IDataRecord dr, int i)
		{
			return dr.GetDateTime(i);
		}
		private static string ToString(this IDataRecord dr, int i)
		{
			return dr.GetString(i);
		}
		private static object ToObject(this IDataRecord dr, int i)
		{
			return dr.GetValue(i);
		}
		#endregion
	}
}
