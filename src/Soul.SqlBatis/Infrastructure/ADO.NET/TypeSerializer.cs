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
        /// 解构器缓存
        /// </summary>
        private readonly static ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>> _deserializers = new ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>>();

        public static Func<IDataRecord, T> CreateSerializer<T>(IDataRecord record)
        {
            return ActivatorUtilities.Create<T>(record);
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


    }
}
