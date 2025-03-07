using Soul.SqlBatis.Databases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
    public interface IDbContextCommand
    {
        int Execute(string sql, object param = null, Action<DbContextCommandOptions> configure = null);
        Task<int> ExecuteAsync(string sql, object param = null, Action<DbContextCommandOptions> configure = null);
        object ExecuteScalar(string sql, object param = null, Action<DbContextCommandOptions> configure = null);
        Task<object> ExecuteScalarAsync(string sql, object param = null, Action<DbContextCommandOptions> configure = null);
        T ExecuteScalar<T>(string sql, object param = null, Action<DbContextCommandOptions> configure = null);
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, Action<DbContextCommandOptions> configure = null);
        List<T> Query<T>(string sql, object param = null, Action<DbContextCommandOptions> configure = null);
        Task<List<T>> QueryAsync<T>(string sql, object param = null, Action<DbContextCommandOptions> configure = null);
        T QueryFirst<T>(string sql, object param = null, Action<DbContextCommandOptions> configure = null);
        Task<T> QueryFirstAsync<T>(string sql, object param = null, Action<DbContextCommandOptions> configure = null);
        DbDataGrid QueryMultiple(string sql, object param = null, Action<DbContextCommandOptions> configure = null);
    }

    public class DbContextCommand : IDbContextCommand
    {
        private readonly DbContext _context;

        public DbContextCommand(DbContext context)
        {
            _context = context;
        }

        public virtual int Execute(string sql, object param = null, Action<DbContextCommandOptions> configure = null)
        {
            return Start(() =>
            {
                using (var command = CreateCommand(sql, param, configure))
                {
                    return command.ExecuteNonQuery();
                }
            });
        }

        public virtual Task<int> ExecuteAsync(string sql, object param = null, Action<DbContextCommandOptions> configure = null)
        {
            return StartAsync(() =>
            {
                using (var command = CreateAsyncCommand(sql, param, configure))
                {
                    return command.ExecuteNonQueryAsync();
                }
            });
        }

        public virtual object ExecuteScalar(string sql, object param = null, Action<DbContextCommandOptions> configure = null)
        {
            return Start(() =>
            {
                using (var command = CreateCommand(sql, param, configure))
                {
                    return command.ExecuteScalar();
                }
            });
        }

        public virtual Task<object> ExecuteScalarAsync(string sql, object param = null, Action<DbContextCommandOptions> configure = null)
        {
            return StartAsync(() =>
            {
                using (var command = CreateAsyncCommand(sql, param, configure))
                {
                    return command.ExecuteScalarAsync();
                }
            });
        }

        public virtual T ExecuteScalar<T>(string sql, object param = null, Action<DbContextCommandOptions> configure = null)
        {
            return Start(() =>
            {
                using (var command = CreateCommand(sql, param, configure))
                {
                    var result = command.ExecuteScalar();
                    return ChangeType<T>(result);
                }
            });
        }

        public virtual Task<T> ExecuteScalarAsync<T>(string sql, object param = null, Action<DbContextCommandOptions> configure = null)
        {
            return StartAsync(async () =>
            {
                using (var command = CreateAsyncCommand(sql, param, configure))
                {
                    var result = await command.ExecuteScalarAsync();
                    return ChangeType<T>(result);
                }
            });
        }

        public virtual List<T> Query<T>(string sql, object param = null, Action<DbContextCommandOptions> configure = null)
        {
            return Start(() =>
            {
                var list = new List<T>();
                using (var command = CreateCommand(sql, param, configure))
                using (var reader = command.ExecuteReader())
                {
                    var mapper = _context.EntityMapper.CreateMapper<T>(reader);
                    while (reader.Read())
                    {
                        var entity = mapper(reader);
                        list.Add(entity);
                    }
                }
                return list;
            });
        }

        public virtual Task<List<T>> QueryAsync<T>(string sql, object param = null, Action<DbContextCommandOptions> configure = null)
        {
            return StartAsync(async () =>
            {
                var list = new List<T>();
                using (var command = CreateAsyncCommand(sql, param, configure))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var mapper = _context.EntityMapper.CreateMapper<T>(reader);
                    while (reader.Read())
                    {
                        var entity = mapper(reader);
                        list.Add(entity);
                    }
                }
                return list;
            });
        }

        public virtual T QueryFirst<T>(string sql, object param = null, Action<DbContextCommandOptions> configure = null)
        {
            return Start(() =>
            {
                using (var command = CreateAsyncCommand(sql, param, configure))
                using (var reader = command.ExecuteReader())
                {
                    var mapper = _context.EntityMapper.CreateMapper<T>(reader);
                    while (reader.Read())
                    {
                        var entity = mapper(reader);
                        return entity;
                    }
                    return default;
                }
            });
        }

        public virtual Task<T> QueryFirstAsync<T>(string sql, object param = null, Action<DbContextCommandOptions> configure = null)
        {
            return StartAsync(async () =>
            {
                using (var command = CreateAsyncCommand(sql, param, configure))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var mapper = _context.EntityMapper.CreateMapper<T>(reader);
                    while (reader.Read())
                    {
                        var entity = mapper(reader);
                        return entity;
                    }
                    return default;
                }
            });
        }

        public DbDataGrid QueryMultiple(string sql, object param = null, Action<DbContextCommandOptions> configure = null)
        {
            return new DbDataGrid(_context, () => CreateCommand(sql, param, configure), _context.EntityMapper);
        }

        public DbCommand CreateAsyncCommand(string sql, object param, Action<DbContextCommandOptions> configure)
        {
            return CreateCommand(sql, param, configure) as DbCommand
                ?? throw new InvalidCastException("Failed to cast the created command to DbCommand. Please ensure that the CreateCommand method returns a valid DbCommand object.");
        }

        public IDbCommand CreateCommand(string sql, object param, Action<DbContextCommandOptions> configure)
        {
            _context.WriteLog(sql, param);
            var commandOptions = new DbContextCommandOptions();
            configure?.Invoke(commandOptions);
            var command = _context.GetDbConnection().CreateCommand();
            command.CommandText = sql;
            command.Transaction = _context.GetDbTransaction();
            command.CommandType = commandOptions.CommandType;
            command.CommandTimeout = commandOptions.CommandTimeout;
            if (param != null)
            {
                if (param is DynamicParameters dynamicParameters)
                {
                    foreach (var item in dynamicParameters)
                    {
                        SetParameter(command, item.Key, item.Value);
                    }
                }
                else
                {
                    foreach (var item in new DynamicParameters(param))
                    {
                        SetParameter(command, item.Key, item.Value);
                    }
                }
            }
            return command;
        }

        private void SetParameter(IDbCommand command, string name, object value)
        {
            if (!command.CommandText.Contains("@" + name.TrimStart('@')))
            {
                return;
            }
            var inQueryPattern = $@"IN\s+(?<name>@*{name})";
            if (Regex.IsMatch(command.CommandText, inQueryPattern, RegexOptions.IgnoreCase))
            {
                command.CommandText = Regex.Replace(command.CommandText, inQueryPattern, match =>
                {
                    var matchName = match.Groups["name"].Value;
                    var values = new List<object>();
                    if (value != null)
                    {
                        foreach (var item in value as IEnumerable)
                        {
                            values.Add(item);
                        }
                    }
                    if (values == null || values.Count == 0)
                    {
                        return $"IN ({_context.Options.EmptyQuerySql})";
                    }
                    else
                    {
                        var splitParamNames = new List<string>();
                        for (var i = 0; i < values.Count; i++)
                        {
                            var splitParamName = $"{matchName}_{i}";
                            var itemParameterValue = values[i];
                            splitParamNames.Add(splitParamName);
                            CreateParameter(command, splitParamName, itemParameterValue);
                        }
                        return $"IN ({string.Join(",", splitParamNames)})";
                    }
                }, RegexOptions.IgnoreCase);
            }
            else
            {
                CreateParameter(command, name, value);
            }
        }

        private void CreateParameter(IDbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.Value = value ?? DBNull.Value;
            if (value is Enum && value!=null)
            {
                parameter.Value = Convert.ToInt32(value);
            }
            parameter.ParameterName = name;
            command.Parameters.Add(parameter);
        }

        private T Start<T>(Func<T> func)
        {
            var connection = _context.GetDbConnection() as DbConnection;
            var closeConnection = false;
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                    closeConnection = true;
                }
                var result = func();
                return result;
            }
            finally
            {
                if (closeConnection)
                {
                    connection.Close();
                }
            }
        }

        private async Task<T> StartAsync<T>(Func<Task<T>> func)
        {
            var connection = _context.GetDbConnection() as DbConnection;
            var closeConnection = false;
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    await connection.OpenAsync();
                    closeConnection = true;
                }
                var result = await func();
                return result;
            }
            finally
            {
                if (closeConnection)
                {
                    connection.Close();
                }
            }
        }

        private T ChangeType<T>(object value)
        {
            if (value == null || value is DBNull)
            {
                return default;
            }
            var type = typeof(T);
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            var valueType = value.GetType();
            if (valueType == type || type.IsAssignableFrom(valueType))
            {
                return (T)value;
            }
            return (T)Convert.ChangeType(value, underlyingType);
        }
    }
}
