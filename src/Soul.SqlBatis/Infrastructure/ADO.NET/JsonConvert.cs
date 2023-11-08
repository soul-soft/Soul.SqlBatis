using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Soul.SqlBatis.Infrastructure
{
	internal class JsonConverter
	{

		public static bool IsJsonType(Type type)
		{
			return type.CustomAttributes.Any(a => a.AttributeType == typeof(JsonValueAttribute));
		}

		public static string Serialize(object obj)
		{
			return JsonSerializer.Serialize(obj, SqlMapper.Settings.JsonSerializerOptions);
		}

		public static T Deserialize<T>(string json)
		{
			if (json == null)
			{
				if (SqlMapper.Settings.InitializeJsonArray && typeof(IJsonArray).IsAssignableFrom(typeof(T)))
				{
					json = "[]";
				}
				else
				{
					return default;
				}
			}
			return JsonSerializer.Deserialize<T>(json, SqlMapper.Settings.JsonSerializerOptions);
		}

		public static MethodInfo GetDeserializeConverter(Type type)
		{
			return typeof(JsonConverter).GetMethod(nameof(Deserialize), new Type[] { typeof(string) }).MakeGenericMethod(type);
		}
	}
}
