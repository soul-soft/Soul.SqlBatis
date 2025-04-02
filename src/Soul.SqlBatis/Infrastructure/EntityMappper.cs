using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Soul.SqlBatis.Infrastructure
{
    internal class EntityMappper
    {
        private readonly SqlSettings _settings;

        private static readonly ConcurrentDictionary<string, Delegate> _mappers = new ConcurrentDictionary<string, Delegate>();

        public EntityMappper(SqlSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// 创建实体映射器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record"></param>
        /// <returns></returns>
        public Func<IDataRecord, T> CreateMapper<T>(IDataRecord record)
        {
            var bindings = GetEntityMemberBindings(typeof(T), record);
            var cacheKey = GetEntityMapperCacheKey(typeof(T), bindings);
            return _mappers.GetOrAdd(cacheKey, _ =>
            {
                if (bindings.BindType == EntityBindingType.TypeMapper)
                {
                    var lambda = CreateTypeMapperExpression<T>(bindings[0]);
                    return lambda.Compile();
                }
                else if (bindings.BindType == EntityBindingType.MemberBindngs)
                {
                    var lambda = CreateMemberBindingsExpression<T>(bindings);
                    return lambda.Compile();
                }
                else
                {
                    var lambda = CreateConstructorExpression<T>(bindings);
                    return lambda.Compile();
                }
            }) as Func<IDataRecord, T>;
        }

        /// <summary>
        /// 创建类型映射表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindings"></param>
        /// <returns></returns>
        private Expression<Func<IDataRecord, T>> CreateTypeMapperExpression<T>(EntityBinding bindings)
        {
            var parameter = Expression.Parameter(typeof(IDataRecord));
            var expression = CreateNullableValueExpression(parameter, bindings);
            return Expression.Lambda<Func<IDataRecord, T>>(expression, parameter);
        }

        /// <summary>
        /// 创建成员字段绑定表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindings"></param>
        /// <returns></returns>
        private Expression<Func<IDataRecord, T>> CreateMemberBindingsExpression<T>(EntityBindings bindings)
        {
            var parameter = Expression.Parameter(typeof(IDataRecord));
            var memberBindings = new List<MemberBinding>();
            foreach (var item in bindings)
            {
                var expression = CreateNullableValueExpression(parameter, item);
                memberBindings.Add(Expression.Bind(item.Member.AsPropertyInfo(), expression));
            }
            var body = Expression.MemberInit(Expression.New(typeof(T)), memberBindings);
            return Expression.Lambda<Func<IDataRecord, T>>(body, parameter);
        }

        /// <summary>
        /// 创建构造器表达式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindings"></param>
        /// <returns></returns>
        private Expression<Func<IDataRecord, T>> CreateConstructorExpression<T>(EntityBindings bindings)
        {
            var parameter = Expression.Parameter(typeof(IDataRecord));
            var arguments = new List<Expression>();
            foreach (var item in bindings)
            {
                var expression = CreateNullableValueExpression(parameter, item);
                arguments.Add(expression);
            }
            var body = Expression.New(bindings.Constructor, arguments);
            return Expression.Lambda<Func<IDataRecord, T>>(body, parameter);
        }

        /// <summary>
        /// 创建处理DbNull的获取数据表达式
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="binding"></param>
        /// <returns></returns>
        private Expression CreateNullableValueExpression(ParameterExpression parameter, EntityBinding binding)
        {
            var test = Expression.Call(parameter, typeof(IDataRecord).GetMethod(nameof(IDataRecord.IsDBNull)), Expression.Constant(binding.Field.Index));
            var ifFalse = CreateValueExpression(parameter, binding);
            if (ifFalse.Type != binding.Member.Type)
            {
                ifFalse = Expression.Convert(ifFalse, binding.Member.Type);
            }
            var ifTrue = CreateNullableValueExpression(binding.Member.Type);
            return Expression.Condition(test, ifTrue, ifFalse);
        }

        /// <summary>
        /// 创建获取数据表达式
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="binding"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        private Expression CreateValueExpression(ParameterExpression parameter, EntityBinding binding)
        {
            var underlyPropertyType = GetUnderlyingType(binding.Member.Type);
            if (underlyPropertyType.IsEnum)
            {
                var method = typeof(DefaultTypeMapper).GetMethod(nameof(DefaultTypeMapper.EnumTypeMapper)).MakeGenericMethod(underlyPropertyType);
                return Expression.Call(method, parameter, Expression.Constant(binding.Field.Index));
            }
            if (_settings.HasTypeMapper(underlyPropertyType))
            {
                var mapper = _settings.GetTypeMapper(underlyPropertyType);
                return Expression.Invoke(Expression.Constant(mapper), parameter, Expression.Constant(binding.Field.Index));
            }
            else
            {
                throw new NotSupportedException($"Mapping from field '{binding.Field.Name}' of type '{binding.Field.Type}' to member '{binding.Member.Name}' of type '{binding.Member.Type}' is not supported.");
            }
        }

        /// <summary>
        /// 处理空值映射
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Expression CreateNullableValueExpression(Type type)
        {
            var nullMapper = _settings.GetDbNullMapper(GetUnderlyingType(type));
            if (nullMapper == null)
            {
                return Expression.Default(type);
            }
            var expression = Expression.Constant(nullMapper);
            if (expression.Type != type)
            {
                return Expression.Convert(expression, type);
            }
            return expression;
        }

        /// <summary>
        /// 创建映射器缓存key
        /// </summary>
        /// <param name="bindings"></param>
        /// <returns></returns>
        private string CreateEntityMapperCacheKey(Type entityType, EntityBindings bindings)
        {
            var sb = new StringBuilder();
            if (bindings.BindType == EntityBindingType.TypeMapper)
            {
                sb.Append($"{bindings[0].Field.Type.FullName}");
            }
            else
            {
                sb.AppendLine($"{entityType.FullName}");
                foreach (var item in bindings)
                {
                    sb.AppendLine($"{item.Field.Name}:{item.Field.Type.FullName}|{item.Member.Name}:{item.Member.Type.FullName}");
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
        private EntityBindings GetEntityMemberBindings(Type entityType, IDataRecord record)
        {
            var underlyingEntityType = GetUnderlyingType(entityType);
            var constructors = entityType.GetConstructors();
            if (_settings.HasTypeMapper(underlyingEntityType) && record.FieldCount == 1)
            {
                //如果存在映射器
                var bidnings = new List<EntityBinding>();
                var fieldName = record.GetName(0);
                var fieldType = record.GetFieldType(0);
                var filedInfo = new EntityFieldInfo(fieldName, fieldType, 0);
                var memberInfo = new EntityMemberInfo(string.Empty, entityType, null);
                bidnings.Add(new EntityBinding(filedInfo, memberInfo));
                return new EntityBindings(entityType, EntityBindingType.TypeMapper, bidnings);
            }
            else if (constructors.Any(a => a.GetParameters().Length == 0))
            {
                //如果存在无参构造
                var bidnings = new List<EntityBinding>();
                for (int i = 0; i < record.FieldCount; i++)
                {
                    var fieldName = record.GetName(i);
                    var member = FindEntityMemberInfo(entityType, fieldName);
                    if (member == null)
                    {
                        continue;
                    }
                    var fieldType = record.GetFieldType(i);
                    var filedInfo = new EntityFieldInfo(fieldName, fieldType, i);
                    var memberInfo = new EntityMemberInfo(member.Name, member.PropertyType, member);
                    bidnings.Add(new EntityBinding(filedInfo, memberInfo));
                }
                return new EntityBindings(entityType, EntityBindingType.MemberBindngs, bidnings);
            }
            else
            {
                var bidnings = new List<EntityBinding>();
                var constructor = entityType.GetConstructors().OrderByDescending(a => a.GetParameters().Length).First();
                for (int i = 0; i < record.FieldCount; i++)
                {
                    var fieldName = record.GetName(i);
                    var parameter = FindEntityParameterInfo(constructor, fieldName);
                    if (parameter == null)
                    {
                        continue;
                    }
                    var fieldType = record.GetFieldType(i);
                    var filedInfo = new EntityFieldInfo(fieldName, fieldType, i);
                    var memberInfo = new EntityMemberInfo(parameter.Name, parameter.ParameterType, parameter);
                    bidnings.Add(new EntityBinding(filedInfo, memberInfo));
                }

                return new EntityBindings(entityType, EntityBindingType.Constructor, bidnings, constructor);
            }
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
                    || name.Replace("_", string.Empty).Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 查找构造器中匹配的参数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private ParameterInfo FindEntityParameterInfo(ConstructorInfo constructor, string name)
        {
            foreach (var item in constructor.GetParameters())
            {
                if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                    || name.Replace("_", string.Empty).Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// 生成映射器缓存key
        /// </summary>
        /// <param name="bindings"></param>
        /// <returns></returns>
        private string GetEntityMapperCacheKey(Type entityTye,EntityBindings bindings)
        {
            if (bindings.BindType == EntityBindingType.TypeMapper)
            {
                return $"{bindings[0].Field.Type}->{entityTye.FullName}|{bindings.BindType}";
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine($"{entityTye.FullName}|{bindings.Count}|{bindings.BindType}");
                foreach (var item in bindings)
                {
                    sb.Append($"{item.Field.Name}[{item.Field.Index}]->{item.Member.Name}|");
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 移除掉Nullable类型嵌套
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Type GetUnderlyingType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }
    }

    public struct EntityFieldInfo
    {
        public string Name { get; }
        public Type Type { get; }
        public int Index { get; }

        public EntityFieldInfo(string fieldName, Type fieldType, int fieldIndex)
        {
            Name = fieldName;
            Type = fieldType;
            Index = fieldIndex;
        }
    }

    public struct EntityMemberInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public object Data { get; set; }

        public EntityMemberInfo(string name, Type type, object data)
        {
            Name = name;
            Type = type;
            Data = data;
        }

        public MemberInfo AsPropertyInfo()
        {
            return Data as PropertyInfo;
        }
    }

    public class EntityBinding
    {
        public EntityFieldInfo Field { get; }
        public EntityMemberInfo Member { get; }
        public EntityBinding(EntityFieldInfo field, EntityMemberInfo member)
        {
            Field = field;
            Member = member;
        }
    }

    public class EntityBindings : List<EntityBinding>
    {
        public Type EntityType { get; }

        public EntityBindingType BindType { get; }

        public ConstructorInfo Constructor { get; }

        public EntityBindings(Type entityType, EntityBindingType bindingType, IEnumerable<EntityBinding> bindings) : base(bindings)
        {
            EntityType = entityType;
            BindType = bindingType;
        }

        public EntityBindings(Type entityType, EntityBindingType bindingType, IEnumerable<EntityBinding> bindings, ConstructorInfo constructor) : this(entityType, bindingType, bindings)
        {
            Constructor = constructor;
        }
    }

    public enum EntityBindingType
    {
        TypeMapper,
        Constructor,
        MemberBindngs,
    }

    public static class DefaultTypeMapper
    {
        public static T EnumTypeMapper<T>(IDataRecord record, int index)
        {
            var value = record.GetInt32(index);
            if (value is T result)
            {
                return result;
            }
            return (T)Enum.Parse(typeof(T), value.ToString());
        }
    }
}
