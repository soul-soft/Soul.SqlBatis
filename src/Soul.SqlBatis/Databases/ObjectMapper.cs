using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Soul.SqlBatis
{
    public static class ObjectMapper
    {
        private static readonly ConcurrentDictionary<Type, Delegate> _mappers = new ConcurrentDictionary<Type, Delegate>();

        public static Func<object, Dictionary<string, object>> CreateMapper(Type type)
        {
            return (Func<object, Dictionary<string, object>>)_mappers.GetOrAdd(type, key => 
            {
                return GenerateMapper(type);
            });
        }

        private static Func<object, Dictionary<string, object>> GenerateMapper(Type type)
        {
            // 创建一个参数表达式，表示输入的对象
            var parameter = Expression.Parameter(typeof(object), "obj");

            // 将参数转换为指定的类型
            var castedObject = Expression.Convert(parameter, type);

            // 创建一个字典的初始化表达式
            var dictionaryType = typeof(Dictionary<string, object>);
            var dictionaryConstructor = dictionaryType.GetConstructor(Type.EmptyTypes);
            var dictionaryInit = Expression.New(dictionaryConstructor);

            // 获取所有公共属性
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 创建一个字典的初始化表达式
            var addMethod = dictionaryType.GetMethod("Add");

            // 使用 ListInit 来添加属性到字典
            var initializers = new List<ElementInit>();

            foreach (var property in properties)
            {
                var propertyValue = Expression.Property(castedObject, property);
                var key = Expression.Constant(property.Name);
                var value = Expression.Convert(propertyValue, typeof(object));

                // 创建一个元素初始化表达式
                var elementInit = Expression.ElementInit(addMethod, key, value);
                initializers.Add(elementInit);
            }

            // 创建一个成员初始化表达式
            var dictionaryInitExpression = Expression.ListInit(
                dictionaryInit,
                initializers
            );

            // 创建并返回一个委托
            return Expression.Lambda<Func<object, Dictionary<string, object>>>(dictionaryInitExpression, parameter).Compile();
        }
    }
}
