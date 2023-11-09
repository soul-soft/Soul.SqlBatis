using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public static class TypeSerializer
    {
        private readonly static ConcurrentDictionary<string, Delegate> _serializers = new ConcurrentDictionary<string, Delegate>();
        private readonly static ConcurrentDictionary<Type, Delegate> _deserializers = new ConcurrentDictionary<Type, Delegate>();
        private readonly static ConcurrentDictionary<string, Delegate> _serializerMappers = new ConcurrentDictionary<string, Delegate>();


        #region Deserializer
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
            return (Func<object, Dictionary<string, object>>)_deserializers.GetOrAdd(type, key =>
            {
                return CreateDeserializerDelegate(key);
            });
        }

        private static Func<object, Dictionary<string, object>> CreateDeserializerDelegate(Type type)
        {
            var parameter = Expression.Parameter(typeof(object), "p");
            var instanceParameter = Expression.Convert(parameter, type);
            var instance = Expression.New(typeof(Dictionary<string, object>).GetConstructor(Type.EmptyTypes));
            var addMethod = typeof(Dictionary<string, object>).GetMethod(nameof(Dictionary<string, object>.Add));
            var elements = new List<ElementInit>();
            foreach (var item in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                var key = Expression.Constant(item.Name);
                var value = Expression.MakeMemberAccess(instanceParameter, item) as Expression;
                if (item.PropertyType != typeof(object))
                {
                    value = Expression.Convert(value, typeof(object));
                }
                elements.Add(Expression.ElementInit(addMethod, key, value));
            }
            var body = Expression.ListInit(instance, elements);
            var lambda = Expression.Lambda(body, parameter);
            return lambda.Compile() as Func<object, Dictionary<string, object>>;
        }
        #endregion

        #region Serializer
        public static Func<IDataRecord, dynamic> CreateDynamicSerializer()
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

        public static Func<IDataRecord, T> CreateSerializer<T>(IDataRecord dr)
        {
            var fields = dr.GetFields();
            var key = CreateSerializerDelegateKey<T>(fields);
            return (Func<IDataRecord, T>)_serializers.GetOrAdd(key, k =>
            {
                return CreateSerializerDelegate<T>(dr);
            });
        }

        public static void AddSerializerMapper<TColumn, TMember>(Func<TColumn, TMember> converter)
        {
            var key = CreateSerializerMapperKey(typeof(TColumn), typeof(TMember));
            _serializerMappers.TryAdd(key, converter);
        }
        #endregion

        private static Func<IDataRecord, T> CreateSerializerDelegate<T>(IDataRecord dr)
        {
            var fields = dr.GetFields();
            var entityType = typeof(T);
            var parameter = Expression.Parameter(typeof(IDataRecord), "dr");
            if (fields.Count == 1 && IDataRecordExtensions.GetGetMethod(entityType) != null)
            {
                var body = CreateSerializerExpression(parameter, entityType, fields[0]);
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
                    var property = FindEntityMember(properties, item.Name);
                    if (property == null)
                    {
                        continue;
                    }
                    var bind = Expression.Bind(property, CreateSerializerExpression(parameter, property.PropertyType, item));
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
                    arguments.Add(CreateSerializerExpression(parameter, memberType, item));
                }
                var body = Expression.New(constructor2, arguments);
                var lambda = Expression.Lambda(body, parameter);
                return (Func<IDataRecord, T>)lambda.Compile();
            }
        }

        internal static bool TryGetSerializerMapper(Type columnType, Type memberType, out Delegate mapper)
        {
            var key = CreateSerializerMapperKey(columnType, memberType);
            return _serializerMappers.TryGetValue(key, out mapper);
        }



        private static Expression CreateSerializerExpression(Expression parameter, Type memberType, DataRecordField field)
        {
            try
            {
                var isDbNullMethod = typeof(IDataRecord).GetMethod(nameof(IDataRecord.IsDBNull), new Type[] { typeof(int) });
                var ordinalExpression = Expression.Constant(field.Ordinal);
                var test = Expression.Call(parameter, isDbNullMethod, ordinalExpression);
                var ifTrue = Expression.Default(memberType);
                Expression ifElse;
                var getMethod = IDataRecordExtensions.GetGetMethod(field.Type);
                if (getMethod.IsStatic)
                {
                    ifElse = Expression.Call(getMethod, parameter, ordinalExpression);
                }
                else
                {
                    ifElse = Expression.Call(parameter, getMethod, ordinalExpression);
                }
                if (memberType != field.Type)
                {
                    if (TryGetSerializerMapper(field.Type, memberType, out Delegate mapper))
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
                        var converter = GetObjectToStringMethod(field.Type);
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
            catch(Exception ex)
            {
                throw new InvalidCastException($"Unable to cast object of type '{field.Type}' to type '{memberType}'. On the '{field.Name}' column.", ex);
            }
        }

        private static string CreateSerializerMapperKey(Type columnType, Type memberType)
        {
            return $"{columnType.GUID:N}|{memberType.GUID:N}";
        }

        private static string CreateSerializerDelegateKey<T>(List<DataRecordField> fields)
        {
            var entityType = typeof(T);
            if (fields.Count == 1 && IDataRecordExtensions.GetGetMethod(entityType) != null)
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

        private static MethodInfo GetObjectToStringMethod(Type type)
        {
            return typeof(Convert).GetMethod(nameof(Convert.ToString), new Type[] { type });
        }
    }
}
