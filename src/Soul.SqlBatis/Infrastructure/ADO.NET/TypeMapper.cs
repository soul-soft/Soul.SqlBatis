using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public static class TypeMapper
	{
		private static ConcurrentDictionary<string, Delegate> _mappers = new ConcurrentDictionary<string, Delegate>();

		public static void AddMapper<TColumn, TMember>(Func<TColumn, TMember> converter)
		{
			var key = CreateKey(typeof(TColumn), typeof(TMember));
			_mappers.TryAdd(key, converter);
		}

		internal static bool TryGetMapper(Type columnType, Type memberType, out Delegate mapper)
		{
			var key = CreateKey(columnType, memberType);
			return _mappers.TryGetValue(key, out mapper);
		}

		internal static MethodInfo GetStringMapper(Type memberType)
		{
			return typeof(Convert).GetMethod(nameof(Convert.ToString), new Type[] { memberType });
		}

		private static string CreateKey(Type columnType, Type memberType)
		{
			return $"{columnType.GUID:N}|{memberType.GUID:N}";
		}
	}
}
