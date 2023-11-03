using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	internal static class DefaultDataConverter
	{
		private readonly static List<MethodInfo> _converters;
		
		static DefaultDataConverter()
		{
			_converters = typeof(DefaultDataConverter).GetMethods().ToList();
		}

		public static bool TryGetConverter(Type type,out MethodInfo converter)
		{
			converter = _converters.Where(a => a.ReturnType == type).FirstOrDefault();
			return converter != null;
		}

		#region Converters
		public static byte ToByte(this IDataRecord dr, int i)
		{
			return dr.GetByte(i);
		}
		public static byte[] ToBytes(this IDataRecord dr, int i)
		{
			var buffer = new byte[SqlMapper.Settings.BinaryBufferSize];
			var length = dr.GetBytes(i, 0, buffer, 0, buffer.Length);
			return buffer.Take((int)length).ToArray();
		}
		public static char ToChar(this IDataRecord dr, int i)
		{
			return dr.GetChar(i);
		}
		public static char[] ToChars(this IDataRecord dr, int i)
		{
			var buffer = new char[SqlMapper.Settings.TextBufferSize];
			var length = dr.GetChars(i, 0, buffer, 0, buffer.Length);
			return buffer.Take((int)length).ToArray();
		}
		public static bool ToBoolean(this IDataRecord dr, int i)
		{
			return dr.ToBoolean(i);
		}
		public static short ToInt16(this IDataRecord dr, int i)
		{
			return dr.GetInt16(i);
		}
		public static int ToInt32(this IDataRecord dr, int i)
		{
			return dr.GetInt32(i);
		}
		public static long ToInt64(this IDataRecord dr, int i)
		{
			return dr.GetInt64(i);
		}
		public static float GetFloat(this IDataRecord dr, int i)
		{
			return dr.GetFloat(i);
		}
		public static double GetDouble(this IDataRecord dr, int i)
		{
			return dr.GetDouble(i);
		}
		public static decimal GetDecimal(this IDataRecord dr, int i)
		{
			return dr.GetDecimal(i);
		}
		public static Guid GetGuid(this IDataRecord dr, int i)
		{
			return dr.GetGuid(i);
		}
		public static DateTime GetDateTime(this IDataRecord dr, int i)
		{
			return dr.GetDateTime(i);
		}
		public static string ToString(this IDataRecord dr, int i)
		{
			return dr.GetString(i);
		}
		public static object ToObject(this IDataRecord dr, int i)
		{
			return dr.GetValue(i);
		}
		#endregion
		
	}
}
