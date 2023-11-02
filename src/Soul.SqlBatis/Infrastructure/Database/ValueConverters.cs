using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis
{
	internal class ValueConverters
	{
		private static List<ValueConverter> _converters;

		static ValueConverters()
		{
			_converters = typeof(ValueConverters).GetMethods()
				.Where(a => a.CustomAttributes.Any(c => c.AttributeType == typeof(ValueConverterAttribute)))
				.Select(s =>
				{
					var converterMethodAttribute = s.GetCustomAttribute<ValueConverterAttribute>();
					return new ValueConverter(converterMethodAttribute.Token, converterMethodAttribute.IsNullable, s);
				})
				.ToList();
		}

		#region FindValueConverterMethod
		/// <summary>
		/// 查找转换器
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="InvalidCastException"></exception>
		public static MethodInfo FindValueConverterMethod(Type type)
		{
			ValueConverter converter;
			if (ReflectionUtility.GetUnderlyingType(type).IsEnum)
			{
				converter = _converters
					.Where(a => a.Token == ValueConverterToken.Enum)
					.Where(a => (ReflectionUtility.IsNullable(type) == a.IsNullable) || (ReflectionUtility.IsNullable(type) == a.IsNullable))
					.First();
			}
			else if (TypeSerializer.IsJsonType(type))
			{
				converter = _converters
					.Where(a => a.Token == ValueConverterToken.Json)
					.First();
			}
			else
			{
				converter = _converters
					.Where(a => a.Method.ReturnType == type)
					.FirstOrDefault();
			}
			if (converter == null)
			{
				var message = string.Format("No converter found for {0} type", type.Name);
				throw new InvalidCastException(message);
			}
			var method = converter.Method;
			if (method.IsGenericMethod)
			{
				return method.MakeGenericMethod(type);
			}
			return method;
		}

		/// <summary>
		/// 是否存在转换器
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool HasValueConverterMethod(Type type)
		{
			if (ReflectionUtility.GetUnderlyingType(type).IsEnum)
			{
				return true;
			}
			if (TypeSerializer.IsJsonType(type))
			{
				return true;
			}
			return _converters.Any(a => a.Method.ReturnType == type);
		}
		#endregion

		#region Reference Type Converts
		[ValueConverter(IsNullable = true)]
		public static object ToObject(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return dr.GetValue(i);
			}
			catch
			{
				throw ThrowInvalidCastException<object>(dr, i);
			}
		}
		[ValueConverter(IsNullable = true)]
		public static string ToString(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return dr.GetString(i);
			}
			catch
			{
				throw ThrowInvalidCastException<string>(dr, i);
			}
		}
		[ValueConverter(IsNullable = true)]
		public static byte[] ToBytes(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				var buffer = new byte[SqlMapper.Settings.BinaryBufferSize];
				var length = dr.GetBytes(i, 0, buffer, 0, buffer.Length);
				return buffer.Take((int)length).ToArray();
			}
			catch
			{
				throw ThrowInvalidCastException<byte>(dr, i);
			}
		}
		[ValueConverter(Token = ValueConverterToken.Json, IsNullable = true)]
		public static T ToJson<T>(IDataRecord dr, int i)
		{
			var json = ToString(dr, i);
			return TypeSerializer.JsonDeserialize<T>(json);
		}
		#endregion

		#region Value Type Converts
		[ValueConverter]
		public static byte ToByte(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				var result = dr.GetByte(i);
				return result;
			}
			catch
			{
				throw ThrowInvalidCastException<byte>(dr, i);
			}
		}
		[ValueConverter]
		public static sbyte ToSByte(IDataRecord dr, int i)
		{
			var result = ToByte(dr, i);
			return Convert.ToSByte(result);
		}
		[ValueConverter]
		public static short ToInt16(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return dr.GetInt16(i);
			}
			catch
			{
				throw ThrowInvalidCastException<short>(dr, i);
			}
		}
		[ValueConverter]
		public static ushort ToUInt16(IDataRecord dr, int i)
		{
			var result = ToInt16(dr, i);
			return Convert.ToUInt16(result);
		}
		[ValueConverter]
		public static int ToInt32(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return dr.GetInt32(i);
			}
			catch
			{
				throw ThrowInvalidCastException<int>(dr, i);
			}
		}
		[ValueConverter]
		public static uint ToUInt32(IDataRecord dr, int i)
		{
			var result = ToInt32(dr, i);
			return Convert.ToUInt32(result);
		}
		[ValueConverter]
		public static long ToInt64(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return dr.GetInt64(i);
			}
			catch
			{
				throw ThrowInvalidCastException<long>(dr, i);
			}
		}
		[ValueConverter]
		public static ulong ToUInt64(IDataRecord dr, int i)
		{
			var result = ToInt64(dr, i);
			return Convert.ToUInt64(result);
		}
		[ValueConverter]
		public static float ToFloat(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return dr.GetFloat(i);
			}
			catch
			{
				throw ThrowInvalidCastException<float>(dr, i);
			}
		}
		[ValueConverter]
		public static double ToDouble(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return dr.GetDouble(i);
			}
			catch
			{
				throw ThrowInvalidCastException<double>(dr, i);
			}
		}
		[ValueConverter]
		public static bool ToBoolean(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return dr.GetBoolean(i);
			}
			catch
			{
				throw ThrowInvalidCastException<bool>(dr, i);
			}
		}
		[ValueConverter]
		public static decimal ToDecimal(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return dr.GetDecimal(i);
			}
			catch
			{
				throw ThrowInvalidCastException<bool>(dr, i);
			}
		}
		[ValueConverter]
		public static char ToChar(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				var result = dr.GetChar(i);
				return result;
			}
			catch
			{
				throw ThrowInvalidCastException<char>(dr, i);
			}
		}
		[ValueConverter]
		public static DateTime ToDateTime(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return dr.GetDateTime(i);
			}
			catch
			{
				throw ThrowInvalidCastException<DateTime>(dr, i);
			}
		}
		[ValueConverter(Token = ValueConverterToken.Enum)]
		public static T ToEnum<T>(IDataRecord dr, int i)
			where T : struct
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				var value = dr.GetValue(i);
				if (Enum.TryParse(value.ToString(), out T result))
					return result;
				return default;
			}
			catch
			{
				throw ThrowInvalidCastException<T>(dr, i);
			}
		}
		[ValueConverter]
		public static Guid ToGuid(IDataRecord dr, int i)
		{
			try
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				var result = dr.GetGuid(i);
				return result;
			}
			catch
			{
				throw ThrowInvalidCastException<Guid>(dr, i);
			}
		}
		#endregion

		#region Nullable Value Type Converts
		[ValueConverter(IsNullable = true)]
		public static byte? ToNullableByte(IDataRecord dr, int i)
		{
			if (dr.IsDBNull(i))
			{
				return default;
			}
			return ToByte(dr, i);
		}
		[ValueConverter(IsNullable = true)]
		public static sbyte? ToNullableSByte(IDataRecord dr, int i)
		{
			var result = ToNullableByte(dr, i);
			if (result == null)
			{
				return default;
			}
			return Convert.ToSByte(result.Value);
		}
		[ValueConverter(IsNullable = true)]
		public static short? ToNullableInt16(IDataRecord dr, int i)
		{
			if (dr.IsDBNull(i))
			{
				return default;
			}
			return ToInt16(dr, i);
		}
		[ValueConverter(IsNullable = true)]
		public static ushort? ToNullableUInt16(IDataRecord dr, int i)
		{
			var result = ToNullableInt16(dr, i);
			if (result == null)
			{
				return default;
			}
			return Convert.ToUInt16(result.Value);
		}
		[ValueConverter(IsNullable = true)]
		public static int? ToNullableInt32(IDataRecord dr, int i)
		{
			if (dr.IsDBNull(i))
			{
				return default;
			}
			return ToInt32(dr, i);
		}
		[ValueConverter(IsNullable = true)]
		public static uint? ToNullableUInt32(IDataRecord dr, int i)
		{
			var result = ToNullableInt32(dr, i);
			if (result == null)
			{
				return default;
			}
			return Convert.ToUInt32(result.Value);
		}
		[ValueConverter(IsNullable = true)]
		public static long? ToNullableInt64(IDataRecord dr, int i)
		{
			if (dr.IsDBNull(i))
			{
				return default;
			}
			return ToInt64(dr, i);
		}
		[ValueConverter(IsNullable = true)]
		public static ulong? ToNullableUInt64(IDataRecord dr, int i)
		{
			var result = ToNullableInt64(dr, i);
			if (result == null)
			{
				return default;
			}
			return Convert.ToUInt64(result.Value);
		}
		[ValueConverter(IsNullable = true)]
		public static float? ToNullableFloat(IDataRecord dr, int i)
		{
			if (dr.IsDBNull(i))
			{
				return default;
			}
			return ToFloat(dr, i);
		}
		[ValueConverter(IsNullable = true)]
		public static double? ToNullableDouble(IDataRecord dr, int i)
		{
			if (dr.IsDBNull(i))
			{
				return default;
			}
			return ToDouble(dr, i);
		}
		[ValueConverter(IsNullable = true)]
		public static bool? ToNullableBoolean(IDataRecord dr, int i)
		{
			if (dr.IsDBNull(i))
			{
				return default;
			}
			return ToBoolean(dr, i);
		}
		[ValueConverter(IsNullable = true)]
		public static decimal? ToNullableDecimal(IDataRecord dr, int i)
		{
			if (dr.IsDBNull(i))
			{
				return default;
			}
			return ToDecimal(dr, i);
		}
		[ValueConverter(IsNullable = true)]
		public static char? ToNullableChar(IDataRecord dr, int i)
		{
			if (dr.IsDBNull(i))
			{
				return default;
			}
			return ToChar(dr, i);
		}
		[ValueConverter(IsNullable = true)]
		public static DateTime? ToNullableDateTime(IDataRecord dr, int i)
		{
			if (dr.IsDBNull(i))
			{
				return default;
			}
			return ToDateTime(dr, i);
		}
		[ValueConverter(IsNullable = true, Token = ValueConverterToken.Enum)]
		public static T? ToNullableEnum<T>(IDataRecord dr, int i)
			where T : struct, Enum
		{
			if (dr.IsDBNull(i))
			{
				return default;
			}
			return ToEnum<T>(dr, i);
		}
		[ValueConverter(IsNullable = true)]
		public static Guid? ToNullableGuid(IDataRecord dr, int i)
		{
			if (dr.IsDBNull(i))
			{
				return default;
			}
			return ToGuid(dr, i);
		}
		#endregion

		#region Throw InvalidCastException
		private static Exception ThrowInvalidCastException<T>(IDataRecord dr, int i)
		{
			var column = dr.GetName(i);
			var fieldType = dr.GetFieldType(i);
			return new InvalidCastException($"Unable to cast object of type '{fieldType}' to type '{typeof(T)}' at the column '{column}'.");
		}
		#endregion
	}

	internal class ValueConverter
	{
		public ValueConverterToken Token { get; set; }
		public bool IsNullable { get; set; }
		public MethodInfo Method { get; set; }

		public ValueConverter(ValueConverterToken token, bool isNullable, MethodInfo method)
		{
			Token = token;
			IsNullable = isNullable;
			Method = method;
		}
	}
	/// <summary>
	/// 用于标记转换器
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	internal class ValueConverterAttribute : Attribute
	{
		public ValueConverterToken Token { get; set; }
		public bool IsNullable { get; set; }
	}
	/// <summary>
	/// 标记转换的类型
	/// </summary>
	internal enum ValueConverterToken
	{
		Enum,
		Json
	}
}
