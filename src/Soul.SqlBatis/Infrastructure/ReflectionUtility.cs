using System;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	internal static class ReflectionUtility
	{
		/// <summary>
		/// 是否是Nullable类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsNullable(Type type)
		{
			return Nullable.GetUnderlyingType(type) != null;
		}
		/// <summary>
		/// 获取非Nullable类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Type GetUnderlyingType(Type type)
		{
			return Nullable.GetUnderlyingType(type) ?? type;
		}
		/// <summary>
		/// 获取无参构造器
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		public static ConstructorInfo GetNonParameterConstructor(Type entityType)
		{
			var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			return entityType.GetConstructor(flags, null, Type.EmptyTypes, null);
		}
		/// <summary>
		/// 获取无参构造器
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="constructor"></param>
		/// <returns></returns>
		public static bool TryGetNonParameterConstructor(Type entityType, out ConstructorInfo constructor)
		{
			constructor = GetNonParameterConstructor(entityType);
			return constructor != null;
		}
		/// <summary>
		/// 获取参数最长的构造器
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		public static ConstructorInfo GetMaxParameterConstructor(Type entityType)
		{
			var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
			return entityType.GetConstructors(flags).OrderByDescending(a => a.GetParameters().Length).First();
		}
	}
}
