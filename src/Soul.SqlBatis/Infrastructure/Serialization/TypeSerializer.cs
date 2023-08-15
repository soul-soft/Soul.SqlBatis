using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.Json;

namespace Soul.SqlBatis
{
    public static class TypeSerializer
    {
        /// <summary>
        /// 序列器缓存
        /// </summary>
        private static ConcurrentDictionary<string, Delegate> _serializers = new ConcurrentDictionary<string, Delegate>();
        /// <summary>
        /// 解构器缓存
        /// </summary>
        private static ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>> _deserializers = new ConcurrentDictionary<Type, Func<object, Dictionary<string, object>>>();

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonSerialize(object obj)
        {
            return JsonSerializer.Serialize(obj, SqlMapper.Settings.JsonSerializerOptions);
        }
        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, SqlMapper.Settings.JsonSerializerOptions);
        }
        /// <summary>
        /// 是否是json类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsJsonValueType(Type type)
        {
            return type.CustomAttributes.Any(a => a.AttributeType == typeof(JsonValueAttribute));
        }
        /// <summary>
        /// 创建序列器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Func<IDataRecord, T> CreateSerializer<T>(IDataRecord record)
        {
            var key = CreateEmitSerializerCacheKey(typeof(T), record);
            return _serializers.GetOrAdd(key, _ =>
            {
                return CreateEmitSerializer<T>(record);
            }) as Func<IDataRecord, T>;
        }
        /// <summary>
        /// 创建动态类型序列器
        /// </summary>
        /// <returns></returns>
        public static Func<IDataRecord, dynamic> CreateSerializer()
        {
            return (record) =>
            {
                var expando = new System.Dynamic.ExpandoObject();
                var entity = (IDictionary<string, dynamic>)expando;
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
        /// <summary>
        /// 创建解构器 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Func<object, Dictionary<string, object>> CreateDeserializer(Type type)
        {
            if (type == typeof(Dictionary<string, object>))
            {
                return (object values) =>
                {
                    return (Dictionary<string, object>)values;
                };
            }
            return _deserializers.GetOrAdd(type, _ =>
            {
                return CreateEmitDeserializer(type);
            });
        }
        /// <summary>
        /// 创建对象解构器
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 创建序列器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static Func<IDataRecord, T> CreateEmitSerializer<T>(IDataRecord record)
        {
            var entityType = typeof(T);
            if (record.FieldCount == 1 && ValueConverters.HasValueConverterMethod(entityType))
            {
                var dynamicMethod = new DynamicMethod($"Adpt", entityType, new Type[] { typeof(IDataRecord) }, true);
                var generator = dynamicMethod.GetILGenerator();
                var local = generator.DeclareLocal(entityType);
                var converterMethod = ValueConverters.FindValueConverterMethod(entityType);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldc_I4, 0);
                if (converterMethod.IsVirtual)
                    generator.Emit(OpCodes.Callvirt, converterMethod);
                else
                    generator.Emit(OpCodes.Call, converterMethod);
                if (entityType == typeof(object) && converterMethod.ReturnType.IsValueType)
                {
                    generator.Emit(OpCodes.Box, converterMethod.ReturnType);
                }
                generator.Emit(OpCodes.Stloc, local);
                generator.Emit(OpCodes.Ldloc, local);
                generator.Emit(OpCodes.Ret);
                return (Func<IDataRecord, T>)dynamicMethod.CreateDelegate(typeof(Func<IDataRecord, T>));
            }
            else if (GetNonParameterConstructor(entityType) != null)
            {
                var constructor = GetNonParameterConstructor(entityType);
                var dynamicMethod = new DynamicMethod($"Adpt", entityType, new Type[] { typeof(IDataRecord) }, true);
                var generator = dynamicMethod.GetILGenerator();
                var entityReference = generator.DeclareLocal(entityType);
                generator.Emit(OpCodes.Newobj, constructor);
                generator.Emit(OpCodes.Stloc, entityReference);
                var recordFields = GetDataRecordFields(record);
                foreach (var item in recordFields)
                {
                    var property = FindEntityTypePropery(entityType, item.Name);
                    if (property == null)
                    {
                        continue;
                    }
                    var converterMethod = ValueConverters.FindValueConverterMethod(property.PropertyType);
                    generator.Emit(OpCodes.Ldloc, entityReference);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, item.Ordinal);
                    if (converterMethod.IsVirtual)
                        generator.Emit(OpCodes.Callvirt, converterMethod);
                    else
                        generator.Emit(OpCodes.Call, converterMethod);
                    generator.Emit(OpCodes.Callvirt, property.SetMethod);
                }
                generator.Emit(OpCodes.Ldloc, entityReference);
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(Func<IDataRecord, T>)) as Func<IDataRecord, T>;
            }
            else
            {
                var dynamicMethod = new DynamicMethod($"Adpt", entityType, new Type[] { typeof(IDataRecord) }, true);
                var generator = dynamicMethod.GetILGenerator();
                var constructor = entityType.GetConstructors()
                    .OrderByDescending(a => a.GetParameters().Length)
                    .First();
                var parameters = constructor.GetParameters().ToList();
                var parameterReferences = parameters
                    .Select(s => generator.DeclareLocal(s.ParameterType))
                    .ToArray();
                var entityReference = generator.DeclareLocal(entityType);
                var recordFields = GetDataRecordFields(record);
                foreach (var item in recordFields)
                {
                    var parameter = FindConstructorInfoParameter(constructor, item.Name);
                    int parameterIndex = parameters.IndexOf(parameter);
                    var converterMethod = ValueConverters.FindValueConverterMethod(item.Type);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldc_I4, item.Ordinal);
                    if (converterMethod.IsVirtual)
                        generator.Emit(OpCodes.Callvirt, converterMethod);
                    else
                        generator.Emit(OpCodes.Call, converterMethod);
                    generator.Emit(OpCodes.Stloc, parameterReferences[parameterIndex]);
                }
                foreach (var item in parameterReferences)
                {
                    generator.Emit(OpCodes.Ldloc, item);
                }
                generator.Emit(OpCodes.Newobj, constructor);
                generator.Emit(OpCodes.Stloc, entityReference);
                generator.Emit(OpCodes.Ldloc, entityReference);
                generator.Emit(OpCodes.Ret);
                return dynamicMethod.CreateDelegate(typeof(Func<IDataRecord, T>)) as Func<IDataRecord, T>;
            }
        }
        /// <summary>
        /// 创建序列化缓存key
        /// </summary>
        /// <returns></returns>
        private static string CreateEmitSerializerCacheKey(Type type, IDataRecord record)
        {
            var fields = GetDataRecordFields(record).Select(s =>
            {
                if (SqlMapper.Settings.MatchNamesWithUnderscores)
                {
                    return s.Name.ToUpper();
                }
                else
                {
                    return s.Name.ToUpper().Replace("_", string.Empty);
                }
            });
            string names = string.Empty;
            if (!ValueConverters.HasValueConverterMethod(type))
            {
                names = string.Join(",", fields);
            }
            return string.Format("{0}|{1}|{2}", type.Name, names, type.GUID.ToString("N"));
        }
        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static PropertyInfo FindEntityTypePropery(Type type, string name)
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
        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="constructor"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        private static ParameterInfo FindConstructorInfoParameter(ConstructorInfo constructor, string name)
        {
            var parameter = constructor.GetParameters()
                .Where(a => a.Name.ToUpper().Equals(name.ToUpper()))
                .FirstOrDefault();
            if (parameter == null)
            {
                var message = string.Format("No parameter found for {0} constructor of type", constructor.DeclaringType);
                throw new InvalidCastException(message);
            }
            return parameter;
        }
        /// <summary>
        /// 获取记录中的字段信息
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private static IEnumerable<DataRecordField> GetDataRecordFields(IDataRecord record)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                var name = record.GetName(i);
                var type = record.GetFieldType(i);
                var ordinal = i;
                yield return new DataRecordField(name, type, ordinal);
            }
        }
        /// <summary>
        /// 获取无参构造器
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private static ConstructorInfo GetNonParameterConstructor(Type entityType)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            return entityType.GetConstructor(flags, null, Type.EmptyTypes, null);
        }
        /// <summary>
        /// IDataRecord信息
        /// </summary>
        class DataRecordField
        {
            public string Name { get; }
            public Type Type { get; }
            public int Ordinal { get; }
            public DataRecordField(string name, Type type, int ordinal)
            {
                Name = name;
                Type = type;
                Ordinal = ordinal;
            }
        }
    }
}
