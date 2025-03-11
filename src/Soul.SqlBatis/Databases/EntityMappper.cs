using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Collections.Concurrent;
using System.Text;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Databases
{
    internal class EntityMappper
    {
        private readonly SqlSettings _options;

        private static readonly ConcurrentDictionary<string, Delegate> _mappers = new ConcurrentDictionary<string, Delegate>();

        public EntityMappper(SqlSettings options)
        {
            _options = options;
        }

        public Func<IDataRecord, T> CreateMapper<T>(IDataRecord record)
        {
            var bindings = GetEntityMemberBindings(typeof(T), record);
            var cacheKey = CreateMapperCacheKey(bindings);
            return _mappers.GetOrAdd(cacheKey, _ =>
            {
                var lambda = CreateMemberBindLambda<T>(bindings);
                return lambda.Compile();
            }) as Func<IDataRecord, T>;
        }

        private Expression<Func<IDataRecord, T>> CreateMemberBindLambda<T>(List<EntityBinding> bindings)
        {
            var parameter = Expression.Parameter(typeof(IDataRecord));
            var memberBindings = new List<MemberBinding>();
            foreach (var item in bindings)
            {
                var test = Expression.Call(parameter, typeof(IDataRecord).GetMethod(nameof(IDataRecord.IsDBNull)), Expression.Constant(item.Field.Index));
                var ifFalse = CreateGetValueExpression(parameter, item);
                var underlyMemberType = GetUnderlyingType(item.Member.PropertyType);
                if (ifFalse.Type != underlyMemberType)
                {
                    ifFalse = Expression.Convert(ifFalse, item.Member.PropertyType);
                }
                var ifTrue = GetDbNullMapperExpression(underlyMemberType);
                var finalExpression = Expression.Condition(test, ifTrue, ifFalse);
                memberBindings.Add(Expression.Bind(item.Member, finalExpression));
            }
            var body = Expression.MemberInit(Expression.New(typeof(T)), memberBindings);
            return Expression.Lambda<Func<IDataRecord, T>>(body, parameter);
        }

        private Expression GetDbNullMapperExpression(Type type)
        {
            var nullValue = _options.GetDbNullMapper(type);
            if (nullValue == null)
            {
                return Expression.Default(type);
            }
            return Expression.Constant(nullValue);
        }

        private Expression CreateGetValueExpression(ParameterExpression parameter, EntityBinding binding)
        {
            var underlyPropertyType = GetUnderlyingType(binding.Member.PropertyType);
            if (underlyPropertyType.IsEnum)
            {
                var method = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt32), new Type[] { typeof(int) });
                return Expression.Call(parameter, method, Expression.Constant(binding.Field.Index));
            }
            var mapper = _options.GetTypeMapper(underlyPropertyType);
            if (mapper != null)
            {
                return Expression.Invoke(Expression.Constant(mapper), parameter, Expression.Constant(binding.Field.Index));
            }
            else
            {
                throw new NotSupportedException($"{binding.Field.Type} to {binding.Member.PropertyType}");
            }
        }



        private string CreateMapperCacheKey(List<EntityBinding> bindings)
        {
            var sb = new StringBuilder();
            if (bindings.Count == 1)
            {
                sb.Append($"{bindings[0].Field.Name}:{bindings[0].Field.Type.FullName}");
            }
            else
            {
                foreach (var item in bindings)
                {
                    sb.AppendLine($"{item.Field.Name}:{item.Field.Type.FullName}|{item.Member.Name}:{item.Member.PropertyType.FullName}");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将字段绑定到实体属性
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private List<EntityBinding> GetEntityMemberBindings(Type entityType, IDataRecord record)
        {
            var bidnings = new List<EntityBinding>();
            for (int i = 0; i < record.FieldCount; i++)
            {
                var member = FindEntityMemberInfo(entityType, record.GetName(i));
                if (member == null)
                {
                    continue;
                }
                var fieldName = record.GetName(i);
                var fieldType = record.GetFieldType(i);
                var filedInfo = new FieldInfo(fieldName, fieldType, i);
                bidnings.Add(new EntityBinding(filedInfo, member));
            }
            return bidnings;
        }

        /// <summary>
        /// 查找实体中匹配的属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private PropertyInfo FindEntityMemberInfo(Type type, string name)
        {
            foreach (var item in type.GetProperties())
            {
                if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                    || item.Name.Replace("_", string.Empty).Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }

        private static Type GetUnderlyingType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }
    }

    public struct FieldInfo
    {
        public string Name { get; }
        public Type Type { get; }
        public int Index { get; }

        public FieldInfo(string fieldName, Type fieldType, int fieldIndex)
        {
            Name = fieldName;
            Type = fieldType;
            Index = fieldIndex;
        }
    }

    public struct EntityBinding
    {
        public FieldInfo Field { get; }
        public PropertyInfo Member { get; }
        public ParameterInfo Parameter { get; }

        public EntityBinding(FieldInfo field, PropertyInfo member)
        {
            Field = field;
            Member = member;
            Parameter = null;
        }

        public EntityBinding(FieldInfo field, ParameterInfo parameter)
        {
            Field = field;
            Member = null;
            Parameter = parameter;
        }
    }
}
