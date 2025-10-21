using System.Collections.Concurrent;
using System.Reflection.Emit;
using System.Reflection;
using System;

namespace Soul.SqlBatis.ChangeTracking
{
    public class EmitProxyGenerator
    {
        private static readonly ConcurrentDictionary<Type, Type> ProxyCache = new ConcurrentDictionary<Type, Type>();

        public static IPropertyAccessor CreateProxy(object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var targetType = target.GetType();
            var proxyType = ProxyCache.GetOrAdd(targetType, GenerateProxyType);
            return (IPropertyAccessor)Activator.CreateInstance(proxyType, target);
        }

        private static Type GenerateProxyType(Type targetType)
        {
            var assemblyName = new AssemblyName("DynamicProxyAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            var typeBuilder = moduleBuilder.DefineType(
            $"{targetType.Name}_Proxy",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(object));

            typeBuilder.AddInterfaceImplementation(typeof(IPropertyAccessor));

            // 定义 _target 字段
            var targetField = typeBuilder.DefineField(
                "_target",
                targetType,
                FieldAttributes.Private | FieldAttributes.InitOnly);

            // 构造函数
            var ctor = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new[] { targetType });

            var il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, targetField);
            il.Emit(OpCodes.Ret);

            ImplementGetPropertyValue(typeBuilder, targetType, targetField);
            ImplementSetPropertyValue(typeBuilder, targetType, targetField);

            return typeBuilder.CreateTypeInfo();
        }

        private static void ImplementGetPropertyValue(TypeBuilder typeBuilder, Type targetType, FieldInfo targetField)
        {
            var method = typeBuilder.DefineMethod(
                "GetPropertyValue",
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(object),
                new[] { typeof(string) });

            var il = method.GetILGenerator();
            var properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var getter = prop.GetGetMethod(true);
                if (getter == null) continue;

                var nextLabel = il.DefineLabel();

                // if (name != prop.Name) goto nextLabel;
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldstr, prop.Name);
                il.Emit(OpCodes.Call, typeof(string).GetMethod("op_Equality", new[] { typeof(string), typeof(string) }));
                il.Emit(OpCodes.Brfalse_S, nextLabel);

                // return _target.Property;
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, targetField);
                il.Emit(OpCodes.Callvirt, getter);
                if (prop.PropertyType.IsValueType)
                    il.Emit(OpCodes.Box, prop.PropertyType);
                il.Emit(OpCodes.Ret);

                il.MarkLabel(nextLabel);
            }

            // Property not found
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Ret);
        }

        private static void ImplementSetPropertyValue(TypeBuilder typeBuilder, Type targetType, FieldInfo targetField)
        {
            var method = typeBuilder.DefineMethod(
                "SetPropertyValue",
                MethodAttributes.Public | MethodAttributes.Virtual,
                null,
                new[] { typeof(string), typeof(object) });

            var il = method.GetILGenerator();
            var properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var setter = prop.GetSetMethod(true);
                if (setter == null) continue;

                var nextLabel = il.DefineLabel();

                // if (name != prop.Name) goto nextLabel;
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldstr, prop.Name);
                il.Emit(OpCodes.Call, typeof(string).GetMethod("op_Equality", new[] { typeof(string), typeof(string) }));
                il.Emit(OpCodes.Brfalse_S, nextLabel);

                // _target.Property = value;
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, targetField);
                il.Emit(OpCodes.Ldarg_2);
                if (prop.PropertyType.IsValueType)
                    il.Emit(OpCodes.Unbox_Any, prop.PropertyType);
                else
                    il.Emit(OpCodes.Castclass, prop.PropertyType);
                il.Emit(OpCodes.Callvirt, setter);
                il.Emit(OpCodes.Ret);

                il.MarkLabel(nextLabel);
            }

            il.Emit(OpCodes.Ret);

        }
    }
}
