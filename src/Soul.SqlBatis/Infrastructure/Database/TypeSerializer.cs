using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
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

		public static Func<IDataRecord, T> CreateSerializer<T>(IDataRecord record)
		{
			var columns = GetDataReaderMetadata(record).ToList();
			var key = CreateEmitSerializerCacheKey(typeof(T), columns);
			return _serializers.GetOrAdd(key, _ =>
			{
				return CreateEmitSerializer<T>(record);
			}) as Func<IDataRecord, T>;
		}
		
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
		
		private static Func<IDataRecord, T> CreateEmitSerializer<T>(IDataRecord record)
		{
			var returnType = typeof(T);
			var columns = GetDataReaderMetadata(record).ToList();
			if (columns.Count() == 1 && TypeMapper.HasDefaultConveter(returnType))
			{
				var parameter = Expression.Parameter(typeof(IDataRecord), "dr");
				var body = BuildConvertExpression(parameter, columns[0].Type, returnType, 0);
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
					var bind = Expression.Bind(property, BuildConvertExpression(parameter, item.Type, property.PropertyType, item.Ordinal));
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
					arguments.Add(BuildConvertExpression(parameter, column.Type, item.ParameterType, column.Ordinal));
				}
				var body = Expression.New(constructor, arguments);
				var lambda = Expression.Lambda(body, parameter);
				return (Func<IDataRecord, T>)lambda.Compile();
			}
		}
		
		private static string CreateEmitSerializerCacheKey(Type type, List<DataReaderColumn> columns)
		{
			if (columns.Count == 1 && TypeMapper.HasDefaultConveter(type))
			{
				return string.Format("{0}|{1}", type.GUID.ToString("N"), columns[0].Type.GUID.ToString("N"));
			}
			return string.Format("{0}|{1}", type.GUID.ToString("N"), columns.Select(s => s.Name), columns.Select(s => s.Type.GUID.ToString("N")));
		}
		
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
		
		private static ConstructorInfo GetNonParameterConstructor(Type entityType)
		{
			var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			return entityType.GetConstructor(flags, null, Type.EmptyTypes, null);
		}
		
		private static bool HasNonParameterConstructor(Type entityType)
		{
			return GetNonParameterConstructor(entityType) != null;
		}

		private static Expression BuildConvertExpression(Expression parameter, Type columnType, Type memberType, int ordinal)
		{
			var test = Expression.Call(parameter, TypeMapper.IsDBNullMethod, Expression.Constant(ordinal));
			var ifTrue = Expression.Default(memberType);
			var defaultConverter = TypeMapper.FindDefaultConverter(columnType);
			var ifElse = (Expression)Expression.Call(parameter, defaultConverter, Expression.Constant(ordinal));
			if (memberType != columnType)
			{
				var customConverter = TypeMapper.FindCustomConvert(columnType, memberType);
				if (customConverter != null)
				{
					if (customConverter.Target != null)
					{
						var instance = Expression.Constant(customConverter.Target);
						ifElse = Expression.Call(instance, customConverter.Method, ifElse);
					}
					else
					{
						ifElse = Expression.Call(customConverter.Method, ifElse);
					}
				}
				else if (memberType == typeof(string))
				{
					var converter = TypeMapper.FindStringConverter(columnType);
					ifElse = Expression.Call(converter, ifElse);
				}
				else if (TypeMapper.IsJsonType(memberType))
				{
					var converter = TypeMapper.FindJsonDeserializeConverter(memberType);
					ifElse = Expression.Call(converter, ifElse);
				}
				else
				{
					ifElse = Expression.Convert(ifElse, memberType);
				}
			}
			return Expression.Condition(test, ifTrue, ifElse);
		}
		
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
