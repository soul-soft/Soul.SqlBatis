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
        private readonly static ConcurrentDictionary<TypeSerializerKey, Delegate> _serializers = new ConcurrentDictionary<TypeSerializerKey, Delegate>();
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
                return CreateTypeSerializer<T>(record);
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

        private static Func<IDataRecord, T> CreateTypeSerializer<T>(IDataRecord record)
        {
            var returnType = typeof(T);
            var columns = GetDataReaderMetadata(record).ToList();
            var parameter = Expression.Parameter(typeof(IDataRecord), "dr");
            if (columns.Count == 1 && TypeMapper.HasDefaultConveter(returnType))
            {
                var body = BuildGetValueExpression(parameter, returnType, columns[0]);
                var lambda = Expression.Lambda(body, parameter);
                return (Func<IDataRecord, T>)lambda.Compile();
            }
            else if (TryGetNonParameterConstructor(returnType, out ConstructorInfo nonParameterConstructor))
            {
                var newExpression = Expression.New(nonParameterConstructor);
                var memberBinds = new List<MemberBinding>();
                foreach (var item in columns)
                {
                    var property = MatchEntityProperyInfo(returnType, item.Name);
                    if (property == null)
                    {
                        continue;
                    }
                    var bind = Expression.Bind(property, BuildGetValueExpression(parameter, property.PropertyType, item));
                    memberBinds.Add(bind);
                }
                var body = Expression.MemberInit(newExpression, memberBinds);
                var lambda = Expression.Lambda(body, parameter);
                return (Func<IDataRecord, T>)lambda.Compile();
            }
            else
            {
                var constructor = GetMaxParameterCountConstructor(returnType);
                var arguments = new List<Expression>();
                foreach (var item in constructor.GetParameters())
                {
                    var column = columns.Where(a => a.Name == item.Name).FirstOrDefault();
                    arguments.Add(BuildGetValueExpression(parameter, item.ParameterType, column));
                }
                var body = Expression.New(constructor, arguments);
                var lambda = Expression.Lambda(body, parameter);
                return (Func<IDataRecord, T>)lambda.Compile();
            }
        }

        private static TypeSerializerKey CreateEmitSerializerCacheKey(Type type, List<DataReaderColumn> columns)
        {
            var columnTypes = columns.Select(s => s.Type).ToArray();
            var memberType = GetMatchEntityMemberTypes(type, columns).ToArray();
            return new TypeSerializerKey(type, columnTypes, memberType);
        }

        private static IEnumerable<Type> GetMatchEntityMemberTypes(Type type, List<DataReaderColumn> columns)
        {
            if (columns.Count == 1)
            {
                yield return type;
            }
            if (TryGetNonParameterConstructor(type, out _))
            {
                foreach (var item in columns)
                {
                    var property = MatchEntityProperyInfo(type, item.Name);
                    if (property != null)
                    {
                        yield return property.PropertyType;
                    }
                }
            }
            else
            {
                var constructor = GetMaxParameterCountConstructor(type);
                foreach (var item in constructor.GetParameters())
                {
                    yield return item.ParameterType;
                }
            }
        }

        private static PropertyInfo MatchEntityProperyInfo(Type type, string name)
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

        private static bool TryGetNonParameterConstructor(Type entityType, out ConstructorInfo constructor)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            constructor = entityType.GetConstructor(flags, null, Type.EmptyTypes, null);
            return constructor != null;
        }

        private static ConstructorInfo GetMaxParameterCountConstructor(Type entityType)
        {
            return entityType.GetConstructors()
                    .OrderByDescending(a => a.GetParameters().Length)
                    .First();
        }

        private static Expression BuildGetValueExpression(Expression parameter, Type memberType, DataReaderColumn column)
        {
            try
            {
                var test = Expression.Call(parameter, TypeMapper.IsDBNullMethod, Expression.Constant(column.Ordinal));
                var ifTrue = Expression.Default(memberType);
                var dataReaderConverter = TypeMapper.FindDataReaderConverter(column.Type);
                var ifElse = (Expression)Expression.Call(parameter, dataReaderConverter, Expression.Constant(column.Ordinal));
                if (memberType != column.Type)
                {
                    if (TypeMapper.TryGetCustomConverter(column.Type, memberType, out TypeConverter customConverter))
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
                        var converter = TypeMapper.FindStringConverter(column.Type);
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
            catch (Exception)
            {
                throw new InvalidCastException($"Unable to cast object of type '{column.Type}' to type '{memberType}'. On the '{column}' column.");
            }
        }

    }
    internal class DataReaderColumn
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

    internal class TypeSerializerKey
    {
        public Type TargetType { get; }

        public Type[] ColumnTypes { get; }

        public Type[] MemberTypes { get; }

        public TypeSerializerKey(Type targetType, Type[] columnTypes, Type[] memberTypes)
        {
            TargetType = targetType;
            ColumnTypes = columnTypes;
            MemberTypes = memberTypes;
        }

        public override int GetHashCode()
        {
            return TargetType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (!(obj is TypeSerializerKey))
            {
                return false;
            }
            var other = (TypeSerializerKey)obj;
            if (TargetType != other.TargetType)
            {
                return false;
            }
            if (ColumnTypes.Length != other.ColumnTypes.Length)
            {
                return false;
            }
            for (int i = 0; i < ColumnTypes.Length; i++)
            {
                if (ColumnTypes[i] != other.ColumnTypes[i])
                {
                    return false;
                }
            }
            if (MemberTypes.Length != other.MemberTypes.Length)
            {
                return false;
            }
            for (int i = 0; i < MemberTypes.Length; i++)
            {
                if (MemberTypes[i] != other.MemberTypes[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
