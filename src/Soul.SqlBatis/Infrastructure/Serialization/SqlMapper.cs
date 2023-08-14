using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
	internal static class SqlMapper
	{
		public static List<dynamic> Query(this IDbConnection connection, string sql, object parameter = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			using (var cmd = CreateDbCommand(connection, sql, parameter, transaction, commandTimeout, commandType))
			using (var reader = cmd.ExecuteReader())
			{
				var list = new List<dynamic>();
				var func = TypeSerializer.CreateSerializer();
				while (reader.Read())
				{
					list.Add(func(reader));
				}
				return list;
			}
		}
		public static async Task<List<dynamic>> QueryAsync(this IDbConnection connection, string sql, object parameter = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			using (var cmd = connection.CreateDbCommand(sql, parameter, transaction, commandTimeout, commandType) as DbCommand)
			using (var reader = await cmd.ExecuteReaderAsync())
			{
				var list = new List<dynamic>();
				var func = TypeSerializer.CreateSerializer();
				while (reader.Read())
				{
					list.Add(func(reader));
				}
				return list;
			}
		}
		public static List<T> Query<T>(this IDbConnection connection, string sql, object parameter = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			using (var cmd = connection.CreateDbCommand(sql, parameter, transaction, commandTimeout, commandType))
			using (var reader = cmd.ExecuteReader())
			{
				var list = new List<T>();
				var func = TypeSerializer.CreateSerializer<T>(reader);
				while (reader.Read())
				{
					list.Add(func(reader));
				}
				return list;
			}
		}
		public static async Task<List<T>> QueryAsync<T>(this IDbConnection connection, string sql, object parameter = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			using (var cmd = connection.CreateDbCommand(sql, parameter, transaction, commandTimeout, commandType) as DbCommand)
			using (var reader = await cmd.ExecuteReaderAsync())
			{
				var list = new List<T>();
				var func = TypeSerializer.CreateSerializer<T>(reader);
				while (await reader.ReadAsync())
				{
					list.Add(func(reader));
				}
				return list;
			}
		}
		public static int Execute(this IDbConnection connection, string sql, object parameter = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			using (var cmd = connection.CreateDbCommand(sql, parameter, transaction, commandTimeout, commandType))
			{
				return cmd.ExecuteNonQuery();
			}
		}
		public static async Task<int> ExecuteAsync(this IDbConnection connection, string sql, object parameter = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			using (var cmd = connection.CreateDbCommand(sql, parameter, transaction, commandTimeout, commandType) as DbCommand)
			{
				return await cmd.ExecuteNonQueryAsync();
			}
		}
		public static object ExecuteScalar(this IDbConnection connection, string sql, object parameter = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			using (var cmd = connection.CreateDbCommand(sql, parameter, transaction, commandTimeout, commandType))
			{
				var result = cmd.ExecuteScalar();
				if (result is DBNull || result == null)
				{
					return default;
				}
				return result;
			}
		}
		public static T ExecuteScalar<T>(this IDbConnection connection, string sql, object parameter = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			using (var cmd = connection.CreateDbCommand(sql, parameter, transaction, commandTimeout, commandType))
			{
				var result = cmd.ExecuteScalar();
				return ChangeType<T>(result);
			}
		}
		public static async Task<T> ExecuteScalarAsync<T>(this IDbConnection connection, string sql, object parameter = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			using (var cmd = connection.CreateDbCommand(sql, parameter, transaction, commandTimeout, commandType) as DbCommand)
			{
				var result = await cmd.ExecuteScalarAsync();
				return ChangeType<T>(result);
			}
		}

		public static IDbCommand CreateDbCommand(this IDbConnection connection, string sql, object param, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			var command = connection.CreateCommand();
			command.Transaction = transaction;
			command.CommandText = sql;
			if (commandTimeout.HasValue)
			{
				command.CommandTimeout = commandTimeout.Value;
			}
			if (commandType.HasValue)
			{
				command.CommandType = commandType.Value;
			}
			if (param != null)
			{
				var func = TypeSerializer.CreateDeserializer(param.GetType());
				foreach (var item in func(param))
				{
					if (Regex.IsMatch(command.CommandText, $@"@{item.Key}"))
					{
						command.AddParameter(item.Key, item.Value);
					}
					if (item.Value != null && item.Value is IEnumerable && item.Value.GetType() != typeof(string))
					{
						command.SplitArrayParameter(item.Key, item.Value);
					}
				}
			}
			return command;
		}

		private static T ChangeType<T>(object obj)
		{
			if (obj is DBNull)
			{
				return default;
			}
			if (typeof(T).IsEnum)
			{
				return (T)Enum.Parse(typeof(T), obj.ToString());
			}
			return (T)Convert.ChangeType(obj, typeof(T));
		}

		private static void AddParameter(this IDbCommand command, string name, object value)
		{
			var parameter = command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value ?? DBNull.Value;
			command.Parameters.Add(parameter);
		}

		private static void SplitArrayParameter(this IDbCommand command, string name, object param)
		{
			var names = new List<string>();
			int index = 0;
			foreach (var item in (IEnumerable)param)
			{
				var key = $"{name}_{index}";
				command.AddParameter(key, item);
				names.Add(key);
				index++;
			}
			command.CommandText = Regex.Replace(command.CommandText, $@"(?<=IN\s+)@{name}", match =>
			{
				if (!names.Any())
				{
					return $"({Settings.EmptyArrayQueryCommand})";
				}
				return $"({string.Join(",", names.Select(s => $"@{s}"))})";
			}, RegexOptions.IgnoreCase);
		}

		public static class Settings
		{
			/// <summary>
			/// 匹配名称和下划线
			/// </summary>
			public static bool MatchNamesWithUnderscores { get; set; } = true;
			/// <summary>
			/// 分割空数组的查询语句
			/// </summary>
			public static string EmptyArrayQueryCommand { get; set; } = "SELECT NULL";
			/// <summary>
			/// 二进制缓冲大小
			/// </summary>
			public static int BinaryBufferSize { get; set; } = 1024 * 1024 * 10;
		}
	}
}
