using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public static class ActivatorUtilities
	{
		private readonly static ConcurrentDictionary<EntityKey, Delegate> _converters = new ConcurrentDictionary<EntityKey, Delegate>();

		public static Func<IDataRecord, T> Create<T>(IDataRecord record)
		{
			var members = GetEntityMembers(typeof(T), GetDataColumns(record)).ToList();
			var key = new EntityKey(typeof(T), members);
			return (Func<IDataRecord, T>)_converters.GetOrAdd(key, CreateEntityConverter<T>);
		}

		private static Func<IDataRecord, T> CreateEntityConverter<T>(EntityKey key)
		{
			var parameter = Expression.Parameter(typeof(IDataRecord),"dr");
			if (key.Members.Count == 1 && DataConverter.GetConverter(key.Members[0].MemberType) != null)
			{
				var body = CreateBindExpression(parameter, key.Members[0]);
				var lambda = Expression.Lambda(body, parameter);
				return (Func<IDataRecord, T>)lambda.Compile();
			}
			else if(ReflectionUtility.TryGetNonParameterConstructor(key.EntityType,out ConstructorInfo constructor1))
			{
				var newExpression = Expression.New(constructor1);
				var memberBindings = new List<MemberBinding>();
				var properties = key.EntityType.GetProperties();
				foreach (var item in key.Members)
				{
					var property = properties.Where(a => a.Name == item.MemberName).First();
					var bind = Expression.Bind(property, CreateBindExpression(parameter, item));
					memberBindings.Add(bind);
				}
				var body = Expression.MemberInit(newExpression, memberBindings);
				var lambda = Expression.Lambda(body, parameter);
				return (Func<IDataRecord, T>)lambda.Compile();
			}
			else
			{
				var constructor2 = ReflectionUtility.GetNonParameterConstructor(key.EntityType);
				var arguments = new List<Expression>();
				foreach (var item in key.Members)
				{
					arguments.Add(CreateBindExpression(parameter, item));
				}
				var body = Expression.New(constructor2, arguments);
				var lambda = Expression.Lambda(body, parameter);
				return (Func<IDataRecord, T>)lambda.Compile();
			}
		}

		private static Expression CreateBindExpression(Expression parameter, EntityMember member)
		{
			try
			{
				var test = Expression.Call(parameter, DataConverter.IsDBNullMethod, Expression.Constant(member.ColumnOrdinal));
				var ifTrue = Expression.Default(member.MemberType);
				var dataConverter = DataConverter.GetConverter(member.ColumnType);
				var ifElse = (Expression)Expression.Call(parameter, dataConverter, Expression.Constant(member.ColumnOrdinal));
				if (member.MemberType != member.ColumnType)
				{
					if (TypeMapper.TryGetMapper(member.ColumnType, member.MemberType, out Delegate mapper))
					{
						if (mapper.Method.IsStatic)
						{
							ifElse = Expression.Call(mapper.Method, ifElse);
						}
						else
						{
							ifElse = Expression.Call(Expression.Constant(mapper.Target), mapper.Method, ifElse);
						}
					}
					else if (member.MemberType == typeof(string))
					{
						var converter = TypeMapper.GetStringMapper(member.MemberType);
						ifElse = Expression.Call(converter, ifElse);
					}
					else if (JsonConverter.IsJsonType(member.MemberType))
					{
						var converter = JsonConverter.GetDeserializeConverter(member.MemberType);
						ifElse = Expression.Call(converter, ifElse);
					}
					else
					{
						ifElse = Expression.Convert(ifElse, member.MemberType);
					}
				}
				return Expression.Condition(test, ifTrue, ifElse);
			}
			catch
			{
				throw new InvalidCastException($"Unable to cast object of type '{member.ColumnType}' to type '{member.MemberType}'. On the '{member.ColumnName}' column.");
			}
		}

		private static List<DataColumn> GetDataColumns(IDataRecord record)
		{
			var list = new List<DataColumn>();
			for (int i = 0; i < record.FieldCount; i++)
			{
				var name = record.GetName(i);
				var type = record.GetFieldType(i);
				list.Add(new DataColumn(type, name, i));
			}
			return list;
		}

		private static IEnumerable<EntityMember> GetEntityMembers(Type entityType, List<DataColumn> columns)
		{
			var list = new List<EntityMember>();
			if (columns.Count == 1 && DataConverter.GetConverter(columns[0].Type) != null)
			{
				var column = columns[0];
				yield return new EntityMember(entityType, column.Type, column.Ordinal);
			}
			else if (ReflectionUtility.GetNonParameterConstructor(entityType) != null)
			{
				var properties = entityType.GetProperties();
				foreach (var column in columns)
				{
					var member = FindEntityMember(properties, column.Name);
					if (member != null)
					{
						yield return new EntityMember(column.Type, member.PropertyType, column.Name, member.Name, column.Ordinal);
					}
				}
			}
			else
			{
				var constructor = ReflectionUtility.GetMaxParameterConstructor(entityType);
				var parameters = constructor.GetParameters();
				foreach (var member in parameters)
				{
					var column = columns.Where(a => a.Name == member.Name).First();
					yield return new EntityMember(column.Type, member.ParameterType, column.Name, member.Name, column.Ordinal);
				}
			}
		}

		private static PropertyInfo FindEntityMember(PropertyInfo[] properties, string name)
		{
			var propertyName = name.ToUpper();
			if (!SqlMapper.Settings.MatchNamesWithUnderscores)
			{
				propertyName = propertyName.Replace("_", string.Empty);
			}
			return properties.Where(a => a.Name.ToUpper() == propertyName).FirstOrDefault();
		}

		class EntityMember
		{
			public string MemberName { get; }
			
			public Type MemberType { get; }
			
			public string ColumnName { get; }
			
			public Type ColumnType { get; }
			
			public int ColumnOrdinal { get; }
		
			public EntityMember(Type columnType, Type memberType, int columnOrdinal)
			{
				ColumnType = columnType;
				MemberType = memberType;
				ColumnOrdinal = columnOrdinal;
			}
			
			public EntityMember(Type columnType, Type memberType, string columnName, string memberName, int columnOrdinal)
			{
				ColumnType = columnType;
				MemberType = memberType;
				ColumnName = columnName;
				MemberName = memberName;
				ColumnOrdinal = columnOrdinal;
			}
		}

		class DataColumn
		{
			public Type Type { get; }
			public string Name { get; }
			public int Ordinal { get; }
			public DataColumn(Type type, string name, int ordinal)
			{
				Type = type;
				Name = name;
				Ordinal = ordinal;
			}
		}

		class EntityKey
		{
			public Type EntityType { get; }

			public List<EntityMember> Members { get; }

			public EntityKey(Type entityType, List<EntityMember> members)
			{
				EntityType = entityType;
				Members = members;
			}
		}
	}
}
