using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Soul.SqlBatis
{
    public static class EntityConvert
    {
        private readonly static ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>> _deserializers = new ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>>();
      
        public static Func<IDataRecord, dynamic> CreateEntityDynamicSerializer()
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
            return _deserializers.GetOrAdd(type, key =>
            {
                return CreateEntityDeserializer(key);
            });
        }

        private static Func<object, Dictionary<string, object>> CreateEntityDeserializer(Type type)
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
    }
}
