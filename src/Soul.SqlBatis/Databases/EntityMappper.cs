using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Collections.Concurrent;
using System.Text;
using System.Linq.Expressions;
using System.Linq;

namespace Soul.SqlBatis.Databases
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
            var cacheKey = CreateEntityMapperCacheKey(bindings);
            return _mappers.GetOrAdd(cacheKey, _ =>
            {
                if (bindings.BindingType == EntityBindingType.TypeMapper)
                {
                    var lambda = CreateTypeMapperExpression<T>(bindings[0]);
                    return lambda.Compile();
                }
                else if ((bindings.BindingType == EntityBindingType.MemberBindngs))
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
        private Expression<Func<IDataRecord, T>> CreateMemberBindingsExpression<T>(List<EntityBinding> bindings)
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
        private Expression<Func<IDataRecord, T>> CreateConstructorExpression<T>(List<EntityBinding> bindings)
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
        /// 创建处理DbNull的获取数据表达式
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="binding"></param>
        /// <returns></returns>
        private Expression CreateNullableValueExpression(ParameterExpression parameter, EntityBinding binding)
        {
            var test = Expression.Call(parameter, typeof(IDataRecord).GetMethod(nameof(IDataRecord.IsDBNull)), Expression.Constant(binding.Field.Index));
            var ifFalse = CreateValueExpression(parameter, binding);
            var underlyMemberType = GetUnderlyingType(binding.Member.Type);
            if (ifFalse.Type != underlyMemberType)
            {
                ifFalse = Expression.Convert(ifFalse, binding.Member.Type);
            }
            var ifTrue = CreateNullableValueExpression(underlyMemberType);
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
                var method = typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt32), new Type[] { typeof(int) });
                return Expression.Call(parameter, method, Expression.Constant(binding.Field.Index));
            }
            var mapper = _settings.GetTypeMapper(underlyPropertyType);
            if (mapper != null)
            {
                return Expression.Invoke(Expression.Constant(mapper), parameter, Expression.Constant(binding.Field.Index));
            }
            else
            {
                throw new NotSupportedException($"{binding.Field.Type} to {binding.Member.Type}");
            }
        }

        /// <summary>
        /// 处理空值映射
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Expression CreateNullableValueExpression(Type type)
        {
            var nullMapper = _settings.GetDbNullMapper(type);
            if (nullMapper == null)
            {
                return Expression.Default(type);
            }
            return Expression.Constant(nullMapper);
        }

        /// <summary>
        /// 创建映射器缓存key
        /// </summary>
        /// <param name="bindings"></param>
        /// <returns></returns>
        private string CreateEntityMapperCacheKey(EntityBindings bindings)
        {
            var sb = new StringBuilder();
            if (bindings.BindingType == EntityBindingType.TypeMapper)
            {
                sb.Append($"{bindings[0].Field.Name}:{bindings[0].Field.Type.FullName}");
            }
            else
            {
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
            if (_settings.ContainsTypeMapper(underlyingEntityType) && record.FieldCount == 1)
            {
                var bidnings = new List<EntityBinding>();
                var fieldName = record.GetName(0);
                var fieldType = record.GetFieldType(0);
                var filedInfo = new FieldInfo(fieldName, fieldType, 0);
                var memberInfo = new MemberInfo(string.Empty, underlyingEntityType, null);
                bidnings.Add(new EntityBinding(filedInfo, memberInfo));
                return new EntityBindings(EntityBindingType.TypeMapper, bidnings);
            }
            else if (constructors.Any(a => a.GetParameters().Length == 0))
            {
                var bidnings = new List<EntityBinding>();
                for (int i = 0; i < record.FieldCount; i++)
                {
                    var member = FindEntityTypePropertyInfo(entityType, record.GetName(i));
                    if (member == null)
                    {
                        continue;
                    }
                    var fieldName = record.GetName(i);
                    var fieldType = record.GetFieldType(i);
                    var filedInfo = new FieldInfo(fieldName, fieldType, i);
                    var memberInfo = new MemberInfo(member.Name, member.PropertyType, member);
                    bidnings.Add(new EntityBinding(filedInfo, memberInfo));
                }
                return new EntityBindings(EntityBindingType.Constructor, bidnings);
            }
            else
            {
                var bidnings = new List<EntityBinding>();
                for (int i = 0; i < record.FieldCount; i++)
                {
                    var member = FindEntityTypePropertyInfo(entityType, record.GetName(i));
                    if (member == null)
                    {
                        continue;
                    }
                    var fieldName = record.GetName(i);
                    var fieldType = record.GetFieldType(i);
                    var filedInfo = new FieldInfo(fieldName, fieldType, i);
                    var memberInfo = new MemberInfo(member.Name, member.PropertyType, member);
                    bidnings.Add(new EntityBinding(filedInfo, memberInfo));
                }
                return new EntityBindings(EntityBindingType.TypeMapper, bidnings);
            }
        }

        /// <summary>
        /// 查找实体中匹配的属性
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private PropertyInfo FindEntityTypePropertyInfo(Type type, string name)
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

        /// <summary>
        /// 查找构造器中匹配的参数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private PropertyInfo FindEntityTypeParameterInfo(Type type, string name)
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

    public struct MemberInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public object Data { get; set; }

        public MemberInfo(string name, Type type, object data)
        {
            Name = name;
            Type = type;
            Data = data;
        }

        public System.Reflection.MemberInfo AsPropertyInfo()
        {
            return Data as PropertyInfo;
        }
    }

    public struct EntityBinding
    {
        public FieldInfo Field { get; }
        public MemberInfo Member { get; }
        public EntityBinding(FieldInfo field, MemberInfo member)
        {
            Field = field;
            Member = member;
        }
    }

    public class EntityBindings : List<EntityBinding>
    {
        public EntityBindingType BindingType { get; }

        public EntityBindings(EntityBindingType bindingType, IEnumerable<EntityBinding> bindings) : base(bindings)
        {
            BindingType = bindingType;
        }
    }

    public enum EntityBindingType
    {
        TypeMapper,
        Constructor,
        MemberBindngs,
    }
}
