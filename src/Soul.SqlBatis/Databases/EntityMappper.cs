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
        private readonly DbContextOptions _options;

        private static readonly ConcurrentDictionary<string, Delegate> _mappers = new ConcurrentDictionary<string, Delegate>();

        public EntityMappper(DbContextOptions options)
        {
            _options = options;
        }

        public Func<IDataRecord, T> CreateMapper<T>(IDataRecord record)
        {
            var bindings = GetEntityMemberBindings(typeof(T), record);
            var cacheKey = CreateMapperCacheKey(bindings);
            return _mappers.GetOrAdd(cacheKey, CreateMapperDelegate<T>(bindings)) as Func<IDataRecord, T>;
        }

        private Func<IDataRecord, T> CreateMapperDelegate<T>(List<EntityMemberBinding> bindings)
        {
            var parameter = Expression.Parameter(typeof(IDataRecord));
            foreach (var item in bindings)
            {
                var expresssion = CreateGetValueExpression(parameter, item);
            }
        }

        private Expression CreateGetValueExpression(ParameterExpression parameter, EntityMemberBinding binding)
        {
            var underlyPropertyType = Nullable.GetUnderlyingType(binding.PropertyType) ?? binding.PropertyType;
            if (underlyPropertyType.IsEnum)
            {

            }
            var mapper = _options.GetTypeMapper(binding.PropertyType);
            if (mapper != null)
            {
                return Expression.Invoke(Expression.Constant(mapper), parameter, Expression.Constant(binding.FieldIndex));
            }
        }

        private string CreateMapperCacheKey(List<EntityMemberBinding> bindings)
        {
            var sb = new StringBuilder();
            if (bindings.Count == 1)
            {
                sb.Append($"{bindings[0].FieldName}:{bindings[0].FieldType.FullName}");
            }
            else
            {
                foreach (var item in bindings)
                {
                    sb.AppendLine($"{item.FieldName}:{item.FieldType.FullName}|{item.PropertyName}:{item.PropertyType.FullName}");
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
        private List<EntityMemberBinding> GetEntityMemberBindings(Type entityType, IDataRecord record)
        {
            var bidnings = new List<EntityMemberBinding>();
            for (int i = 0; i < record.FieldCount; i++)
            {
                var fieldName = record.GetName(i);
                var fieldType = record.GetFieldType(i);
                var member = FindEntityMemberInfo(entityType, record.GetName(i));
                if (member == null)
                {
                    continue;
                }
                bidnings.Add(new EntityMemberBinding(fieldName, fieldType, i, member.Name, member.DeclaringType));
            }
            return bidnings;
        }

        /// <summary>
        /// 查找实体中匹配的属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private MemberInfo FindEntityMemberInfo(Type type, string name)
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
    }

    public class DefaultTypeMapper
    {
        
    }

    public struct EntityMemberBinding
    {
        public string FieldName { get; }
        public Type FieldType { get; }
        public int FieldIndex { get; }
        public string PropertyName { get; }
        public Type PropertyType { get; }

        public EntityMemberBinding(string fieldName, Type fieldType, int fieldIndex, string propertyName, Type propertyType)
        {
            FieldName = fieldName;
            FieldType = fieldType;
            FieldIndex = fieldIndex;
            PropertyName = propertyName;
            PropertyType = propertyType;
        }
    }
}
