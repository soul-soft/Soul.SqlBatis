using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis
{
	public static class TypeSerializer
	{
		/// <summary>
		/// 序列器缓存
		/// </summary>
		private readonly static ConcurrentDictionary<string, Delegate> _serializers = new ConcurrentDictionary<string, Delegate>();
		/// <summary>
		/// 解构器缓存
		/// </summary>
		private readonly static ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>> _deserializers = new ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>>();


		/// <summary>
		/// 创建序列器
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="record"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static Func<IDataRecord, T> CreateSerializer<T>(IDataRecord record)
		{
			var columns = GetDataReaderMetadata(record).ToList();
			var key = CreateEmitSerializerCacheKey(typeof(T), columns);
			return _serializers.GetOrAdd(key, _ =>
			{
				return CreateEmitSerializer<T>(record);
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
				var obj = new System.Dynamic.ExpandoObject();
				var entity = (IDictionary<string, dynamic>)obj;
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
			if (type == typeof(DynamicParameters))
			{
				return (object param) =>
				{
					return (param as DynamicParameters).ToDictionary();
				};
			}
			if (type == typeof(Dictionary<string, object>))
			{
				return (object param) =>
				{
					return (Dictionary<string, object>)param;
				};
			}
			return _deserializers.GetOrAdd(type, _ =>
			{
				return CreateEmitDeserializer(type);
			});
		}
		/// <summary>
		/// 创建对象解构器
		/// </summary>
		/// <returns></returns>
		private static Func<object, Dictionary<string, object>> CreateEmitDeserializer(Type type)
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
		private static Func<IDataRecord, T> CreateEmitSerializer<T>(IDataRecord record)
		{
			var returnType = typeof(T);
			var columns = GetDataReaderMetadata(record).ToList();
			if (columns.Count() == 1 && TypeMapper.HasDefaultConveter(returnType))
			{
				var parameter = Expression.Parameter(typeof(IDataRecord), "dr");
				var body = BuildExpression(parameter, returnType, columns[0].Type, 0);
				var lambda = Expression.Lambda(body, parameter);
				return (Func<IDataRecord, T>)lambda.Compile();
			}
			else if (HasNonParameterConstructor(returnType))
			{
				var parameter = Expression.Parameter(typeof(IDataRecord), "dr");
				var newExpression = Expression.New(GetNonParameterConstructor(returnType));
				var memberBinds = new List<MemberBinding>();
				foreach (var item in columns)
				{
					var property = FindEntityTypePropery(returnType, item.Name);
					var bind = Expression.Bind(property, BuildExpression(parameter, property.PropertyType, item.Type, item.Ordinal));
					memberBinds.Add(bind);
				}
				var body = Expression.MemberInit(newExpression, memberBinds);
				var lambda = Expression.Lambda(body, parameter);
				return (Func<IDataRecord, T>)lambda.Compile();
			}
			else
			{
				var parameter = Expression.Parameter(typeof(IDataRecord), "dr");
				var constructor = returnType.GetConstructors()
					.OrderByDescending(a => a.GetParameters().Length)
					.First();
				var arguments = new List<Expression>();
				foreach (var item in constructor.GetParameters())
				{
					var column = columns.Where(a => a.Name == item.Name).FirstOrDefault();
					arguments.Add(BuildExpression(parameter, item.ParameterType, column.Type, column.Ordinal));
				}
				var body = Expression.New(constructor, arguments);
				var lambda = Expression.Lambda(body, parameter);
				return (Func<IDataRecord, T>)lambda.Compile();
			}
		}
		/// <summary>
		/// 创建序列化缓存key
		/// </summary>
		/// <returns></returns>
		private static string CreateEmitSerializerCacheKey(Type type, List<DataReaderColumn> columns)
		{
			if (columns.Count == 1 && TypeMapper.HasDefaultConveter(type))
			{
				return string.Format("{0}|{1}", type.GUID.ToString("N"), columns[0].Type.GUID.ToString("N"));
			}
			return string.Format("{0}|{1}", type.GUID.ToString("N"), columns.Select(s => s.Name), columns.Select(s => s.Type.GUID.ToString("N")));
		}
		/// <summary>
		/// 获取字段
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		private static PropertyInfo FindEntityTypePropery(Type type, string name)
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
		/// 获取记录中的字段信息
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		private static IEnumerable<DataReaderColumn> GetDataReaderMetadata(IDataRecord record)
		{
			for (int i = 0; i < record.FieldCount; i++)
			{
				var name = record.GetName(i);
				var type = record.GetFieldType(i);
				var ordinal = i;
				yield return new DataReaderColumn(name, type, ordinal);
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
		/// 是否存在无参构造
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		private static bool HasNonParameterConstructor(Type entityType)
		{
			return GetNonParameterConstructor(entityType) != null;
		}

		private static Expression BuildExpression(Expression parameter, Type memberType, Type columnType, int ordinal)
		{
			var test = Expression.Call(parameter, TypeMapper.IsDBNullMethod, Expression.Constant(ordinal));
			var ifTrue = Expression.Default(memberType);
			var convertMethod = TypeMapper.FindDataRecordConverter(columnType);
			var ifElse = (Expression)Expression.Call(parameter, convertMethod, Expression.Constant(ordinal));
			if (memberType != columnType)
			{
				if (TypeMapper.IsJsonType(memberType))
				{
					var jsonConvertMethod = TypeMapper.FindJsonDeserializeConvert(memberType);
					ifElse = Expression.Call(jsonConvertMethod, ifElse);
				}
				else
				{
					ifElse = Expression.Convert(ifElse, memberType);
				}
			}
			return Expression.Condition(test, ifTrue, ifElse);
		}
		/// <summary>
		/// IDataRecord信息
		/// </summary>
		class DataReaderColumn
		{
			public string Name { get; }
			public Type Type { get; }
			public int Ordinal { get; }
			public DataReaderColumn(string name, Type type, int ordinal)
			{
				Name = name;
				Type = type;
				Ordinal = ordinal;
			}
		}
	}
}
