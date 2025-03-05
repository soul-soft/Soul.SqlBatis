using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Soul.SqlBatis
{
    public interface IEntityMapper
    {
        Func<IDataRecord, T> CreateMapper<T>(IDataRecord record);
    }

    public interface ICustomTypeMapper
    {
        MethodInfo GetTypeMapper(EntityTypeMapperContext context);
    }

    public class EntityTypeMapperContext
    {
        public Type EntityType { get; }
        public Type MemberType { get; }
        public Type FieldType { get; }
        public string FieldTypeName { get; }

        public EntityTypeMapperContext(Type entityType, Type memberType, Type fieldType, string fieldTypeName)
        {
            EntityType = entityType;
            MemberType = memberType;
            FieldType = fieldType;
            FieldTypeName = fieldTypeName;
        }
    }

    public class EntityMapperOptions
    {
        public ICustomTypeMapper CustomTypeMapper { get; set; }

        public bool MatchNamesWithUnderscores { get; set; } = true;

        private readonly Dictionary<Type, Delegate> _delegateMappers = new Dictionary<Type, Delegate>();

        public void UseTypeMapper<TResunt>(Func<IDataRecord, int, TResunt> func)
        {
            var type = Nullable.GetUnderlyingType(typeof(TResunt)) ?? typeof(TResunt);
            _delegateMappers[type] = func;
        }

        public Func<EntityTypeMapperContext, Expression> DbNullHandler { get; set; }

        internal Delegate GetTypeMapper(Type type)
        {
            var unNullableType = Nullable.GetUnderlyingType(type) ?? type;
            _delegateMappers.TryGetValue(unNullableType, out var method);
            return method;
        }

        internal bool TryCustomTypeMapper(EntityTypeMapperContext context, out MethodInfo method)
        {
            if (CustomTypeMapper == null)
            {
                method = null;
                return false;
            }
            method = CustomTypeMapper.GetTypeMapper(context);
            var hasMapperMethod = method != null;
            if (hasMapperMethod)
            {
                if (!method.IsStatic)
                {
                    throw new InvalidOperationException("The type mapper method must be static.");
                }
                var parameters = method.GetParameters();
                if (parameters.Length != 2)
                {
                    throw new InvalidOperationException($"The type mapper method must have exactly 2 parameters, but it has {parameters.Length}.");
                }

                if (parameters[0].ParameterType != typeof(IDataRecord))
                {
                    throw new InvalidOperationException($"The first parameter of the type mapper method must be of type IDataRecord, but it is {parameters[0].ParameterType}.");
                }

                if (parameters[1].ParameterType != typeof(int))
                {
                    throw new InvalidOperationException($"The second parameter of the type mapper method must be of type int, but it is {parameters[1].ParameterType}.");
                }
            }
            return hasMapperMethod;
        }
    }

    internal class EntityMapper : IEntityMapper
    {
        private static readonly ConcurrentDictionary<string, Delegate> _mappers = new ConcurrentDictionary<string, Delegate>();

        internal EntityMapperOptions Options { get; }

        public EntityMapper()
            : this(new EntityMapperOptions())
        {
        }

        public EntityMapper(EntityMapperOptions options)
        {
            Options = options;
        }

        public Func<IDataRecord, T> CreateMapper<T>(IDataRecord record)
        {
            var key = CreateMapperCacheKey<T>(record);
            return (Func<IDataRecord, T>)_mappers.GetOrAdd(key, k => GenerateMapper<T>(record));
        }

        public string CreateMapperCacheKey<T>(IDataRecord record)
        {
            string GetTypeFullName(Type type)
            {
                var nullableType = Nullable.GetUnderlyingType(type);
                return nullableType != null ? $"{nullableType.FullName}?" : type.FullName;
            }

            var sb = new StringBuilder();
            sb.Append($"{GetTypeFullName(typeof(T))}|");

            var properties = typeof(T).GetProperties();
            var constructors = typeof(T).GetConstructors();
            var fieldCount = record.FieldCount;

            if (fieldCount == 1 && HasTypeMapper(typeof(T), record.GetFieldType(0)))
            {
                sb.Append(GetTypeFullName(record.GetFieldType(0)));
            }
            else
            {
                var hasParameterlessConstructor = constructors.Any(c => c.GetParameters().Length == 0);
                for (int i = 0; i < fieldCount; i++)
                {
                    var fieldType = record.GetFieldType(i);
                    var fieldName = record.GetName(i);
                    string memberName;

                    if (hasParameterlessConstructor)
                    {
                        var member = MatchEntityMember(properties, fieldName);
                        memberName = member?.Name ?? string.Empty;
                    }
                    else
                    {
                        var constructor = constructors.OrderByDescending(c => c.GetParameters().Length).First();
                        var parameters = constructor.GetParameters();
                        memberName = MatchEntityMember(parameters, fieldName)?.Name ?? string.Empty;
                    }

                    sb.Append($"{GetTypeFullName(fieldType)}|{fieldName}|{memberName}|{i}|");
                }
            }

            return sb.ToString();
        }

        public Func<IDataRecord, T> GenerateMapper<T>(IDataRecord record)
        {
            var recordParameter = Expression.Parameter(typeof(IDataRecord), "record");
            if (record.FieldCount == 1 && HasTypeMapper(typeof(T), record.GetFieldType(0)))
            {
                var binding = new EntityMemberBinding(typeof(T), null, record.GetFieldType(0), null, 0, record.GetDataTypeName(0));
                var body = BuildValueExpression(recordParameter, typeof(T), binding);
                return Expression.Lambda<Func<IDataRecord, T>>(body, recordParameter).Compile();
            }

            return typeof(T).GetConstructors().Any(c => c.GetParameters().Length == 0)
                ? CreateMapperForParameterlessConstructor<T>(record)
                : CreateMapperForParameterizedConstructor<T>(record);
        }

        protected Func<IDataRecord, T> CreateMapperForParameterlessConstructor<T>(IDataRecord record)
        {
            var properties = typeof(T).GetProperties();
            var memberBindings = new List<MemberBinding>();
            var recordParameter = Expression.Parameter(typeof(IDataRecord), nameof(record));

            for (int i = 0; i < record.FieldCount; i++)
            {
                var fieldName = record.GetName(i);
                var fieldType = record.GetFieldType(i);
                var fieldTypeName = record.GetDataTypeName(i);
                var member = MatchEntityMember(properties, fieldName);
                if (member == null)
                    continue;

                var fieldBinding = new EntityMemberBinding(member.PropertyType, member.Name, fieldType, fieldName, i, fieldTypeName);
                var valueExpression = BuildValueExpression(recordParameter, typeof(T), fieldBinding);
                memberBindings.Add(Expression.Bind(member, valueExpression));
            }
            var body = Expression.MemberInit(Expression.New(typeof(T)), memberBindings);
            return Expression.Lambda<Func<IDataRecord, T>>(body, recordParameter).Compile();
        }

        protected Func<IDataRecord, T> CreateMapperForParameterizedConstructor<T>(IDataRecord record)
        {
            var constructor = typeof(T).GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
            var parameters = constructor.GetParameters();
            var constructorArguments = new List<Expression>();
            var recordParameter = Expression.Parameter(typeof(IDataRecord), nameof(record));
            var bindings = new List<EntityMemberBinding>();
            for (int i = 0; i < record.FieldCount; i++)
            {
                var fieldName = record.GetName(i);
                var fieldType = record.GetFieldType(i);
                var fieldTypeName = record.GetDataTypeName(i);
                var member = MatchEntityMember(parameters, fieldName);
                if (member == null)
                    continue;
                var fieldBinding = new EntityMemberBinding(member.ParameterType, member.Name, fieldType, fieldName, i, fieldTypeName);
                bindings.Add(fieldBinding);
            }
            foreach (var item in parameters)
            {
                var binding = bindings.Where(a => a.MemberName == item.Name).FirstOrDefault();
                if (binding != null)
                {
                    var valueExpression = BuildValueExpression(recordParameter, typeof(T), binding);
                    constructorArguments.Add(valueExpression);
                }
                else
                {
                    constructorArguments.Add(Expression.Default(item.ParameterType));
                }
            }

            var body = Expression.New(constructor, constructorArguments);
            return Expression.Lambda<Func<IDataRecord, T>>(body, recordParameter).Compile();
        }

        protected PropertyInfo MatchEntityMember(PropertyInfo[] properties, string fieldName)
        {
            var patternName = Options.MatchNamesWithUnderscores ? fieldName.Replace("_", string.Empty) : fieldName;
            return properties.FirstOrDefault(a => a.Name.Equals(patternName, StringComparison.OrdinalIgnoreCase));
        }

        protected ParameterInfo MatchEntityMember(ParameterInfo[] parameters, string fieldName)
        {
            var patternName = Options.MatchNamesWithUnderscores ? fieldName.Replace("_", string.Empty) : fieldName;
            return parameters.FirstOrDefault(a => a.Name.Equals(patternName, StringComparison.OrdinalIgnoreCase));
        }

        private Expression BuildValueExpression(ParameterExpression recordParameter, Type entityType, EntityMemberBinding fieldBinding)
        {
            var isDbNullCheck = Expression.Call(recordParameter, nameof(IDataRecord.IsDBNull), null, Expression.Constant(fieldBinding.FieldSort));
            Expression valueExpression;
            var context = new EntityTypeMapperContext(entityType, fieldBinding.MemberType, fieldBinding.FieldType, fieldBinding.FieldTypeName);
            if (Options.CustomTypeMapper != null && Options.TryCustomTypeMapper(context, out MethodInfo typeMethod) == true)
            {
                valueExpression = Expression.Call(typeMethod, recordParameter, Expression.Constant(fieldBinding.FieldSort));
            }
            else if (Options.GetTypeMapper(fieldBinding.MemberType) != null)
            {
                var fun = Options.GetTypeMapper(fieldBinding.MemberType);
                valueExpression = Expression.Invoke(Expression.Constant(fun), recordParameter, Expression.Constant(fieldBinding.FieldSort));
            }
            else
            {
                var defaultTypeMethod = GetDefaultTypeMapper(fieldBinding.FieldType)
                    ?? throw new NotImplementedException($"No mapping implemented from field type {fieldBinding.FieldType} to member type {fieldBinding.MemberType}.");
                valueExpression = Expression.Call(recordParameter, defaultTypeMethod, Expression.Constant(fieldBinding.FieldSort));

                if (!fieldBinding.MemberType.IsAssignableFrom(valueExpression.Type))
                {
                    valueExpression = ConvertValueExpression(valueExpression, fieldBinding.MemberType);
                }
            }

            if (valueExpression.Type != fieldBinding.MemberType)
            {
                valueExpression = Expression.Convert(valueExpression, fieldBinding.MemberType);
            }
            var defaultValueExpression = Options.DbNullHandler(context) 
                ?? Expression.Default(fieldBinding.MemberType);
            return Expression.Condition(isDbNullCheck, defaultValueExpression, valueExpression);
        }

        protected bool HasTypeMapper(Type memberType, Type fieldType)
        {
            return Options.GetTypeMapper(memberType) != null || GetDefaultTypeMapper(fieldType) != null;
        }

        protected MethodInfo GetDefaultTypeMapper(Type fieldType)
        {
            var defaultTypeMappers = new Dictionary<Type, MethodInfo>
            {
                { typeof(sbyte), typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetByte)) },
                { typeof(byte), typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetByte)) },
                { typeof(short), typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt16)) },
                { typeof(int), typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt32)) },
                { typeof(long), typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt64)) },
                { typeof(bool), typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetBoolean)) },
                { typeof(string), typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetString)) },
                { typeof(float), typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetFloat)) },
                { typeof(double), typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDouble)) },
                { typeof(decimal), typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDecimal)) },
                { typeof(DateTime), typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDateTime)) },
                { typeof(Guid), typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetGuid)) },
            };
            defaultTypeMappers.TryGetValue(fieldType, out MethodInfo method);
            return method;
        }

        protected Expression ConvertValueExpression(Expression valueExpression, Type targetType)
        {
            var underlyingTargetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            var underlyingValueType = Nullable.GetUnderlyingType(valueExpression.Type) ?? valueExpression.Type;

            if (underlyingTargetType != underlyingValueType)
            {
                var convertMethod = typeof(Convert).GetMethod($"To{underlyingTargetType.Name}", new[] { underlyingValueType });

                if (convertMethod == null)
                {
                    throw new InvalidCastException($"Cannot convert from type '{underlyingValueType.FullName}' to type '{underlyingTargetType.FullName}'.");
                }
                valueExpression = Expression.Call(convertMethod, valueExpression);
            }

            return valueExpression;
        }

        class EntityMemberBinding
        {
            public Type MemberType { get; }
            public string MemberName { get; }
            public Type FieldType { get; }
            public string FieldName { get; }
            public int FieldSort { get; }
            public string FieldTypeName { get; }

            public EntityMemberBinding(Type memberType, string memberName, Type fieldType, string fieldName, int fieldSort, string fieldTypeName)
            {
                MemberType = memberType;
                MemberName = memberName;
                FieldType = fieldType;
                FieldName = fieldName;
                FieldSort = fieldSort;
                FieldTypeName = fieldTypeName;
            }
        }
    }


}
