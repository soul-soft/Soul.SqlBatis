using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Soul.SqlBatis.Test
{
    public static class Desc
    {
        public static Func<object, Dictionary<string, object>> Create(Type type)
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
