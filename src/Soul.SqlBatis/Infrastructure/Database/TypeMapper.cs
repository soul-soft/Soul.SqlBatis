using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Soul.SqlBatis.Infrastructure
{
    public static class TypeMapper
    {
        #region Default Converter
        internal static MethodInfo IsDBNullMethod = typeof(IDataRecord).GetMethod(nameof(IDataRecord.IsDBNull), new Type[] { typeof(int) });

        internal static MethodInfo FindDataReaderConverter(Type type)
        {
            if (type == typeof(short))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt16), new Type[] { typeof(int) });
            }
            if (type == typeof(int))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt32), new Type[] { typeof(int) });
            }
            if (type == typeof(long))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetInt64), new Type[] { typeof(int) });
            }
            if (type == typeof(float))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetFloat), new Type[] { typeof(int) });
            }
            if (type == typeof(double))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDouble), new Type[] { typeof(int) });
            }
            if (type == typeof(decimal))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDecimal), new Type[] { typeof(int) });
            }
            if (type == typeof(string))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetString), new Type[] { typeof(int) });
            }
            if (type == typeof(DateTime))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetDateTime), new Type[] { typeof(int) });
            }
            if (type == typeof(Guid))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetGuid), new Type[] { typeof(int) });
            }
            if (type == typeof(bool))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetBoolean), new Type[] { typeof(int) });
            }
            if (type == typeof(byte))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetByte), new Type[] { typeof(int) });
            }
            if (type == typeof(byte[]))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetBytes), new Type[]
                {
                    typeof(int), typeof(long), typeof(byte[]), typeof(int), typeof(int)
                });
            }
            if (type == typeof(char))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetChar), new Type[] { typeof(int) });
            }
            if (type == typeof(char[]))
            {
                return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetChars), new Type[]
                {
                    typeof(int), typeof(long), typeof(byte[]), typeof(int), typeof(int)
                });
            }
            return typeof(IDataRecord).GetMethod(nameof(IDataRecord.GetValue), new Type[] { typeof(int) });
        }

        internal static bool HasDefaultConveter(Type type)
        {
            if (IsJsonType(type))
            {
                return true;
            }
            return FindDataReaderConverter(type) != null;
        }

        internal static MethodInfo FindJsonDeserializeConverter(Type type)
        {
            return typeof(TypeMapper).GetMethods().Where(a => a.Name == nameof(JsonDeserialize)).First().MakeGenericMethod(type);
        }

        internal static MethodInfo FindStringConverter(Type type)
        {
            return typeof(Convert).GetMethod(nameof(Convert.ToString), new Type[] { type });
        }

        internal static string JsonSerialize(object obj)
        {
            return JsonSerializer.Serialize(obj, SqlMapper.Settings.JsonSerializerOptions);
        }

        public static T JsonDeserialize<T>(string json)
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

        internal static bool IsJsonType(Type type)
        {
            return type.CustomAttributes.Any(a => a.AttributeType == typeof(JsonValueAttribute));
        }
        #endregion

        #region Custom Converter
        private static ConcurrentDictionary<string, TypeConverter> _customConverters = new ConcurrentDictionary<string, TypeConverter>();

        public static void AddConverter<TColumn, TMember>(Func<TColumn, TMember> converter)
        {
            var key = $"{typeof(TColumn).GUID:N}|{typeof(TMember).GUID:N}";
            _customConverters.TryAdd(key, new TypeConverter
            {
                MemberType = typeof(TMember),
                ColumnType = typeof(TColumn),
                Method = converter.Method,
                Target = converter.Target
            });
        }

        internal static bool TryGetCustomConverter(Type columnType, Type memberType, out TypeConverter converter)
        {
            var key = $"{columnType.GUID:N}|{memberType.GUID:N}";
            return _customConverters.TryGetValue(key, out converter);
        }
        #endregion
    }
}
