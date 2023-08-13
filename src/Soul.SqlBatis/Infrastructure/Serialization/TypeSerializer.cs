using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Soul.SqlBatis
{
	public static class TypeSerializer
	{
		//public static void AddConverter<T>(Func<IDataRecord, int, T> func)
		//{

		//}
		/// <summary>
		/// 序列器缓存
		/// </summary>
		private static ConcurrentDictionary<string, Delegate> _serializers = new ConcurrentDictionary<string, Delegate>();
		/// <summary>
		/// 解构器缓存
		/// </summary>
		private static ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>> _deserializers = new ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>>();
		/// <summary>
		/// 创建序列器
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="record"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static Func<IDataRecord, T> CreateSerializer<T>(IDataRecord record)
		{
			var key = CreateSerializerCacheKey(typeof(T), record);
			return _serializers.GetOrAdd(key, _ =>
			{
				return CreateDynamicSerializer<T>(record);
			}) as Func<IDataRecord, T>;
		}
		/// <summary>
		/// 创建动态类型序列器
		/// </summary>
		/// <returns></returns>
		public static Func<IDataRecord, dynamic> CreateSerializer()
		{
			return (record) =>
			{
				var expando = new System.Dynamic.ExpandoObject();
				var entity = (IDictionary<string, dynamic>)expando;
				for (int i = 0; i < record.FieldCount; i++)
				{
					var name = record.GetName(i);
					if (record.IsDBNull(i))
					{
						return null;
					}
					var value = record.GetValue(i);
					entity.Add(name, value);
				}
				return entity;
			};
		}
		/// <summary>
		/// 创建解构器 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Func<object, Dictionary<string, object>> CreateDeserializer(Type type)
		{
			if (type == typeof(Dictionary<string, object>))
			{
				return (object values) =>
				{
					return (Dictionary<string, object>)values;
				};
			}
			return _deserializers.GetOrAdd(type, _ =>
			{
				return CreateDynamicDeserializer(type);
			});
		}
		/// <summary>
		/// 创建对象解构器
		/// </summary>
		/// <returns></returns>
		public static Func<object, Dictionary<string, object>> CreateDynamicDeserializer(Type type)
		{
			var resultType = typeof(Dictionary<string, object>);
			var properties = type.GetProperties();
			var dynamicMethod = new DynamicMethod("Adpt", resultType, new Type[] { typeof(object) }, true);
			var generator = dynamicMethod.GetILGenerator();
			var resultReference = generator.DeclareLocal(resultType);
			var entityReference = generator.DeclareLocal(type);
			generator.Emit(OpCodes.Newobj, resultType.GetConstructor(Type.EmptyTypes));
			generator.Emit(OpCodes.Stloc, resultReference);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Castclass, type);
			generator.Emit(OpCodes.Stloc, entityReference);
			foreach (var item in properties)
			{
				generator.Emit(OpCodes.Ldloc, resultReference);
				generator.Emit(OpCodes.Ldstr, item.Name);
				generator.Emit(OpCodes.Ldloc, entityReference);
				generator.Emit(OpCodes.Callvirt, item.GetMethod);
				if (item.PropertyType.IsValueType)
				{
					generator.Emit(OpCodes.Box, item.PropertyType);
				}
				var addMethod = resultType.GetMethod(nameof(Dictionary<string, object>.Add), new Type[] { typeof(string), typeof(object) });
				generator.Emit(OpCodes.Callvirt, addMethod);
			}
			generator.Emit(OpCodes.Ldloc, resultReference);
			generator.Emit(OpCodes.Ret);
			return dynamicMethod.CreateDelegate(typeof(Func<object, Dictionary<string, object>>)) as Func<object, Dictionary<string, object>>;
		}
		/// <summary>
		/// 创建序列器
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="record"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static Func<IDataRecord, T> CreateDynamicSerializer<T>(IDataRecord record)
		{
			var entityType = typeof(T);
			if (record.FieldCount == 1 && HasConverter(entityType))
			{
				var dynamicMethod = new DynamicMethod($"Adpt", entityType, new Type[] { typeof(IDataRecord) }, true);
				var generator = dynamicMethod.GetILGenerator();
				var local = generator.DeclareLocal(entityType);
				var converterMethod = FindConverter(entityType);
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldc_I4, 0);
				if (converterMethod.IsVirtual)
					generator.Emit(OpCodes.Callvirt, converterMethod);
				else
					generator.Emit(OpCodes.Call, converterMethod);
				if (entityType == typeof(object) && converterMethod.ReturnType.IsValueType)
				{
					generator.Emit(OpCodes.Box, converterMethod.ReturnType);
				}
				generator.Emit(OpCodes.Stloc, local);
				generator.Emit(OpCodes.Ldloc, local);
				generator.Emit(OpCodes.Ret);
				return (Func<IDataRecord, T>)dynamicMethod.CreateDelegate(typeof(Func<IDataRecord, T>));
			}
			else if (GetNonParameterConstructor(entityType) != null)
			{
				var constructor = GetNonParameterConstructor(entityType);
				var dynamicMethod = new DynamicMethod($"Adpt", entityType, new Type[] { typeof(IDataRecord) }, true);
				var generator = dynamicMethod.GetILGenerator();
				var entityReference = generator.DeclareLocal(entityType);
				generator.Emit(OpCodes.Newobj, constructor);
				generator.Emit(OpCodes.Stloc, entityReference);
				var recordFields = GetDataRecordFields(record);
				foreach (var item in recordFields)
				{
					var property = FindPropery(entityType, item.Name);
					if (property == null)
					{
						continue;
					}
					var converterMethod = FindConverter(property.PropertyType);
					generator.Emit(OpCodes.Ldloc, entityReference);
					generator.Emit(OpCodes.Ldarg_0);
					generator.Emit(OpCodes.Ldc_I4, item.Ordinal);
					if (converterMethod.IsVirtual)
						generator.Emit(OpCodes.Callvirt, converterMethod);
					else
						generator.Emit(OpCodes.Call, converterMethod);
					generator.Emit(OpCodes.Callvirt, property.SetMethod);
				}
				generator.Emit(OpCodes.Ldloc, entityReference);
				generator.Emit(OpCodes.Ret);
				return dynamicMethod.CreateDelegate(typeof(Func<IDataRecord, T>)) as Func<IDataRecord, T>;
			}
			else
			{
				var dynamicMethod = new DynamicMethod($"Adpt", entityType, new Type[] { typeof(IDataRecord) }, true);
				var generator = dynamicMethod.GetILGenerator();
				var constructor = entityType.GetConstructors()
					.OrderByDescending(a => a.GetParameters().Length)
					.First();
				var parameters = constructor.GetParameters().ToList();
				var parameterReferences = parameters
					.Select(s => generator.DeclareLocal(s.ParameterType))
					.ToArray();
				var entityReference = generator.DeclareLocal(entityType);
				var recordFields = GetDataRecordFields(record);
				foreach (var item in recordFields)
				{
					var parameter = FindParameter(constructor, item.Name);
					int parameterIndex = parameters.IndexOf(parameter);
					var converterMethod = FindConverter(item.Type);
					generator.Emit(OpCodes.Ldarg_0);
					generator.Emit(OpCodes.Ldc_I4, item.Ordinal);
					if (converterMethod.IsVirtual)
						generator.Emit(OpCodes.Callvirt, converterMethod);
					else
						generator.Emit(OpCodes.Call, converterMethod);
					generator.Emit(OpCodes.Stloc, parameterReferences[parameterIndex]);
				}
				foreach (var item in parameterReferences)
				{
					generator.Emit(OpCodes.Ldloc, item);
				}
				generator.Emit(OpCodes.Newobj, constructor);
				generator.Emit(OpCodes.Stloc, entityReference);
				generator.Emit(OpCodes.Ldloc, entityReference);
				generator.Emit(OpCodes.Ret);
				return dynamicMethod.CreateDelegate(typeof(Func<IDataRecord, T>)) as Func<IDataRecord, T>;
			}
		}
		/// <summary>
		/// 创建序列化缓存key
		/// </summary>
		/// <returns></returns>
		public static string CreateSerializerCacheKey(Type type, IDataRecord record)
		{
			var fields = GetDataRecordFields(record).Select(s =>
			{
				if (SqlMapper.Settings.MatchNamesWithUnderscores)
				{
					return s.Name.ToUpper();
				}
				else
				{
					return s.Name.ToUpper().Replace("_", string.Empty);
				}
			});
			string names = string.Empty;
			if (!HasConverter(type))
			{
				names = string.Join(",", fields);
			}
			return string.Format("{0}|{1}|{2}", type.Name, names, type.GUID.ToString("N"));
		}
		/// <summary>
		/// 获取转换器
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="InvalidCastException"></exception>
		private static MethodInfo FindConverter(Type type)
		{
			MethodInfo method;
			if (GetUnderlyingType(type).IsEnum)
			{
				method = DefaultConverter.Converters
					.Where(a => a.GetCustomAttribute<ConverterMethodAttribute>().IsEnum)
					.Where(a => (IsNullable(type) == IsNullable(a.ReturnType)) || (!IsNullable(type) == !IsNullable(a.ReturnType)))
					.First();
			}
			else
			{
				method = DefaultConverter.Converters
					.Where(a => a.ReturnType == type)
					.FirstOrDefault();
			}
			if (method == null)
			{
				var message = string.Format("No converter found for {0} type", type.Name);
				throw new InvalidCastException(message);
			}
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
		private static bool HasConverter(Type type)
		{
			if (GetUnderlyingType(type).IsEnum)
			{
				return true;
			}
			else
			{
				return DefaultConverter.Converters
					.Where(a => a.ReturnType == type)
					.Any();
			}
		}
		/// <summary>
		/// 获取字段
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private static PropertyInfo FindPropery(Type type, string name)
		{
			var propertyName = name.ToUpper();
			if (!SqlMapper.Settings.MatchNamesWithUnderscores)
			{
				propertyName = propertyName.Replace("_", string.Empty);
			}
			return type.GetProperties()
				.Where(a => a.Name.ToUpper() == propertyName)
				.FirstOrDefault();
		}
		/// <summary>
		/// 获取参数
		/// </summary>
		/// <param name="constructor"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="InvalidCastException"></exception>
		private static ParameterInfo FindParameter(ConstructorInfo constructor, string name)
		{
			var parameter = constructor.GetParameters()
				.Where(a => a.Name.ToUpper().Equals(name.ToUpper()))
				.FirstOrDefault();
			if (parameter == null)
			{
				var message = string.Format("No parameter found for {0} constructor of type", constructor.DeclaringType);
				throw new InvalidCastException(message);
			}
			return parameter;
		}
		/// <summary>
		/// 获取记录中的字段信息
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		private static IEnumerable<DataRecordField> GetDataRecordFields(IDataRecord record)
		{
			for (int i = 0; i < record.FieldCount; i++)
			{
				var name = record.GetName(i);
				var type = record.GetFieldType(i);
				var ordinal = i;
				yield return new DataRecordField(name, type, ordinal);
			}
		}
		/// <summary>
		/// 获取无参构造器
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		private static ConstructorInfo GetNonParameterConstructor(Type entityType)
		{
			var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			return entityType.GetConstructor(flags, null, Type.EmptyTypes, null);
		}
		/// <summary>
		/// 是否是Nullable类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool IsNullable(Type type)
		{
			return Nullable.GetUnderlyingType(type) != null;
		}
		/// <summary>
		/// 获取非Nullable类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static Type GetUnderlyingType(Type type)
		{
			return Nullable.GetUnderlyingType(type) ?? type;
		}
		/// <summary>
		/// IDataRecord信息
		/// </summary>
		class DataRecordField
		{
			public string Name { get; }
			public Type Type { get; }
			public int Ordinal { get; }
			public DataRecordField(string name, Type type, int ordinal)
			{
				Name = name;
				Type = type;
				Ordinal = ordinal;
			}
		}
		/// <summary>
		/// 默认的转换器
		/// </summary>
		static class DefaultConverter
		{
			public static List<MethodInfo> Converters { get; private set; }

			static DefaultConverter()
			{
				Converters = typeof(DefaultConverter).GetMethods()
					.Where(a => a.CustomAttributes.Any(c => c.AttributeType == typeof(ConverterMethodAttribute)))
					.ToList();
			}

			#region Reference Type Converts
			[ConverterMethod(IsNullable = true)]
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
			[ConverterMethod(IsNullable = true)]
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
			[ConverterMethod]
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
			#endregion

			#region Value Type Converts
			[ConverterMethod]
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
			[ConverterMethod]
			public static sbyte ToSByte(IDataRecord dr, int i)
			{
				var result =ToByte(dr,i);
				return Convert.ToSByte(result);
			}
			[ConverterMethod]
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
			[ConverterMethod]
			public static ushort ToUInt16(IDataRecord dr, int i)
			{
				var result = ToInt16(dr, i);
				return Convert.ToUInt16(result);
			}
			[ConverterMethod]
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
			[ConverterMethod]
			public static uint ToUInt32(IDataRecord dr, int i)
			{
				var result = ToInt32(dr, i);
				return Convert.ToUInt32(result);
			}
			[ConverterMethod]
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
			[ConverterMethod]
			public static ulong ToUInt64(IDataRecord dr, int i)
			{
				var result = ToInt64(dr, i);
				return Convert.ToUInt64(result);
			}
			[ConverterMethod]
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
			[ConverterMethod]
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
			[ConverterMethod]
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
			[ConverterMethod]
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
			[ConverterMethod]
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
			[ConverterMethod]
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
			[ConverterMethod(IsEnum = true)]
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
			[ConverterMethod]
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
			[ConverterMethod(IsNullable = true)]
			public static byte? ToNullableByte(IDataRecord dr, int i)
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return ToByte(dr, i);
			}
			[ConverterMethod(IsNullable = true)]
			public static sbyte? ToNullableSByte(IDataRecord dr, int i)
			{
				var result = ToNullableByte(dr, i);
				if (result == null)
				{
					return default;
				}
				return Convert.ToSByte(result.Value);
			}
			[ConverterMethod(IsNullable = true)]
			public static short? ToNullableInt16(IDataRecord dr, int i)
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return ToInt16(dr, i);
			}
			[ConverterMethod(IsNullable = true)]
			public static ushort? ToNullableUInt16(IDataRecord dr, int i)
			{
				var result = ToNullableInt16(dr, i);
				if (result == null)
				{
					return default;
				}
				return Convert.ToUInt16(result.Value);
			}
			[ConverterMethod(IsNullable = true)]
			public static int? ToNullableInt32(IDataRecord dr, int i)
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return ToInt32(dr, i);
			}
			[ConverterMethod(IsNullable = true)]
			public static uint? ToNullableUInt32(IDataRecord dr, int i)
			{
				var result = ToNullableInt32(dr, i);
				if (result == null)
				{
					return default;
				}
				return Convert.ToUInt32(result.Value);
			}
			[ConverterMethod(IsNullable = true)]
			public static long? ToNullableInt64(IDataRecord dr, int i)
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return ToInt64(dr, i);
			}
			[ConverterMethod(IsNullable = true)]
			public static ulong? ToNullableUInt64(IDataRecord dr, int i)
			{
				var result = ToNullableInt64(dr, i);
				if (result == null)
				{
					return default;
				}
				return Convert.ToUInt64(result.Value);
			}
			[ConverterMethod(IsNullable = true)]
			public static float? ToNullableFloat(IDataRecord dr, int i)
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return ToFloat(dr, i);
			}
			[ConverterMethod(IsNullable = true)]
			public static double? ToNullableDouble(IDataRecord dr, int i)
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return ToDouble(dr, i);
			}
			[ConverterMethod(IsNullable = true)]
			public static bool? ToNullableBoolean(IDataRecord dr, int i)
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return ToBoolean(dr, i);
			}
			[ConverterMethod(IsNullable = true)]
			public static decimal? ToNullableDecimal(IDataRecord dr, int i)
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return ToDecimal(dr, i);
			}
			[ConverterMethod(IsNullable = true)]
			public static char? ToNullableChar(IDataRecord dr, int i)
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return ToChar(dr, i);
			}
			[ConverterMethod(IsNullable = true)]
			public static DateTime? ToNullableDateTime(IDataRecord dr, int i)
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return ToDateTime(dr, i);
			}
			[ConverterMethod(IsNullable = true, IsEnum = true)]
			public static T? ToNullableEnum<T>(IDataRecord dr, int i)
				where T : struct, Enum
			{
				if (dr.IsDBNull(i))
				{
					return default;
				}
				return ToEnum<T>(dr, i);
			}
			[ConverterMethod(IsNullable = true)]
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
		/// <summary>
		/// 用于标记转换器
		/// </summary>
		[AttributeUsage(AttributeTargets.Method)]
		class ConverterMethodAttribute : Attribute
		{
			public bool IsEnum { get; set; }
			public bool IsNullable { get; set; }
		}
	}
}
