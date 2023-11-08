using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public static class TypeMapper
	{
        private readonly static ConcurrentDictionary<string, Delegate> _converters = new ConcurrentDictionary<string, Delegate>();
        private static ConcurrentDictionary<string, Delegate> _customConvert = new ConcurrentDictionary<string, Delegate>();
        private readonly static ConcurrentDictionary<Type, Delegate> _dataRecordDelegates = new ConcurrentDictionary<Type, Delegate>();

		public static void AddCustomMapper<TColumn, TMember>(Func<TColumn, TMember> converter)
		{
			var key = CreateCustomMapperKey(typeof(TColumn), typeof(TMember));
			_customConvert.TryAdd(key, converter);
		}

		internal static bool TryGetCustomMapper(Type columnType, Type memberType, out Delegate mapper)
		{
			var key = CreateCustomMapperKey(columnType, memberType);
			return _customConvert.TryGetValue(key, out mapper);
		}

		private static string CreateCustomMapperKey(Type columnType, Type memberType)
		{
			return $"{columnType.GUID:N}|{memberType.GUID:N}";
		}

        public static Func<IDataRecord, T> CreateEntityMapper<T>(IDataRecord dr)
        {
            var fields = dr.GetFields();
            var key = CreateEntityConverterKey<T>(fields);
            return (Func<IDataRecord, T>)_converters.GetOrAdd(key, (Func<string, Delegate>)(k =>
            {
                return CreateEntityMapperDelegete<T>(dr);
            }));
        }

        private static Func<IDataRecord, T> CreateEntityMapperDelegete<T>(IDataRecord dr)
        {
            var fields = dr.GetFields();
            var entityType = typeof(T);
            var parameter = Expression.Parameter(typeof(IDataRecord), "dr");
            if (fields.Count == 1 && GetDataRecordConverter(entityType) != null || JsonConvert.IsJsonType(entityType))
            {
                var body = CreateBindingExpression(parameter, entityType, fields[0]);
                var lambda = Expression.Lambda(body, parameter);
                return (Func<IDataRecord, T>)lambda.Compile();
            }
            else if (ReflectionUtility.TryGetNonParameterConstructor(entityType, out ConstructorInfo constructor1))
            {
                var properties = entityType.GetProperties();
                var memberBindings = new List<MemberBinding>();
                var newExpression = Expression.New(constructor1);
                foreach (var item in fields)
                {
                    var property = FindEntityMember(properties,item.Name);
                    if (property == null)
                    {
                        continue;
                    }
                    var bind = Expression.Bind(property, CreateBindingExpression(parameter, property.PropertyType, item));
                    memberBindings.Add(bind);
                }
                var body = Expression.MemberInit(newExpression, memberBindings);
                var lambda = Expression.Lambda(body, parameter);
                return (Func<IDataRecord, T>)lambda.Compile();
            }
            else
            {
                var arguments = new List<Expression>();
                var constructor2 = ReflectionUtility.GetNonParameterConstructor(entityType);
                var parameters = constructor2.GetParameters();
                foreach (var item in fields)
                {
                    var memberType = parameters.Where(a => a.Name == item.Name).First().ParameterType;
                    arguments.Add(CreateBindingExpression(parameter, memberType, item));
                }
                var body = Expression.New(constructor2, arguments);
                var lambda = Expression.Lambda(body, parameter);
                return (Func<IDataRecord, T>)lambda.Compile();
            }
        }

        private static Expression CreateBindingExpression(Expression parameter, Type memberType, DataRecordField field)
        {
            try
            {
                var isDbNullMethod = typeof(IDataRecord).GetMethod(nameof(IDataRecord.IsDBNull), new Type[] { typeof(int) });
                var ordinalExpression = Expression.Constant(field.Ordinal);
                var test = Expression.Call(parameter, isDbNullMethod, ordinalExpression);
                var ifTrue = Expression.Default(memberType);
                Expression ifElse;
                var dataRecordDelegate = GetDataRecordConverter(field.Type);
                if (dataRecordDelegate.Method.IsStatic)
                {
                    ifElse = Expression.Call(dataRecordDelegate.Method, parameter, ordinalExpression);
                }
                else
                {
                    ifElse = Expression.Call(parameter, dataRecordDelegate.Method, ordinalExpression);
                }
                if (memberType != field.Type)
                {
                    if (TryGetCustomMapper(field.Type, memberType, out Delegate mapper))
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
                    else if (memberType == typeof(string))
                    {
                        var converter = GetToStringConverter(field.Type);
                        ifElse = Expression.Call(converter, ifElse);
                    }
                    else if (JsonConvert.IsJsonType(memberType))
                    {
                        var converter = JsonConvert.GetDeserializeConverter(memberType);
                        ifElse = Expression.Call(converter, ifElse);
                    }
                    else
                    {
                        ifElse = Expression.Convert(ifElse, memberType);
                    }
                }
                return Expression.Condition(test, ifTrue, ifElse);
            }
            catch
            {
                throw new InvalidCastException($"Unable to cast object of type '{field.Type}' to type '{memberType}'. On the '{field.Name}' column.");
            }
        }

        private static string CreateEntityConverterKey<T>(List<DataRecordField> fields)
        {
            var entityType = typeof(T);
            if (fields.Count == 1 && GetDataRecordConverter(entityType) != null || JsonConvert.IsJsonType(entityType))
            {
                return $"{fields[0].Type.GUID:N}|{entityType.GUID:N}";
            }
            else if (ReflectionUtility.TryGetNonParameterConstructor(entityType, out _))
            {
                var names = string.Join(",", fields.Select(s => s.Name));
                var types = string.Join(",", fields.Select(s => s.Type.GUID.ToString("N")));
                return $"{entityType.GUID:N}|{names}|{types}";
            }
            else
            {
                return $"{entityType.GUID:N}";
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

        private static MethodInfo GetToStringConverter(Type type)
        {
            return typeof(Convert).GetMethod(nameof(Convert.ToString), new Type[] { type });
        }

        private static Delegate GetDataRecordConverter(Type type)
        {
            return _dataRecordDelegates.GetOrAdd(type, key =>
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
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetString))
                    .First()
                    .CreateDelegate(typeof(Func<int, string>));
                }
                if (type == typeof(string))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetString))
                    .First()
                    .CreateDelegate(typeof(Func<int, string>));
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
                if (type == typeof(object))
                {
                    return methods.Where(a => a.Name == nameof(IDataRecord.GetValue))
                    .First()
                    .CreateDelegate(typeof(Func<int, object>));
                }
                return null;
            });
        }
    }
}
