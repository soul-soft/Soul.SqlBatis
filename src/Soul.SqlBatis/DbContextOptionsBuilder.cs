using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Soul.SqlBatis
{
    public class DbContextOptionsBuilder
    {
        private readonly DbContextOptions _options;

        private static readonly ConcurrentDictionary<ConnectionProvider, ConnectionFactory> _connectionProvider = new ConcurrentDictionary<ConnectionProvider, ConnectionFactory>();

        public DbContextOptionsBuilder()
        {
            _options = new DbContextOptions();
        }

        internal DbContextOptionsBuilder(DbContextOptions options)
        {
            _options = options;
        }

        public DbContextOptionsBuilder EnableTracking()
        {
            _options.EnableTracking = true;
            return this;
        }

        public DbContextOptionsBuilder EnableNoTracking()
        {
            _options.EnableTracking = false;
            return this;
        }

        public DbContextOptionsBuilder UseMySql(string connectionString)
        {
            var connectionFactory = GetConnectionFactory(ConnectionProvider.MySql);
            connectionFactory.Invoke(connectionString);
            _options.ConnectionFactory = connectionFactory;
            _options.ConnectionString = connectionString;
            return this;
        }

        public DbContextOptionsBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            _options.LoggerFactory = loggerFactory;
            return this;
        }

        public DbContextOptions Build()
        {
            return _options;
        }

        public static void AddConnectionFactory(ConnectionProvider provider, ConnectionFactory connectionFactory)
        {
            _connectionProvider.TryAdd(provider, connectionFactory);
        }

        private static ConnectionFactory GetConnectionFactory(ConnectionProvider provider)
        {
            return _connectionProvider.GetOrAdd(provider, key =>
            {
                Type connectionType = null;
                if (provider == ConnectionProvider.MySql)
                {
                    var assembly = LoadAssembly("MySqlConnector");
                    if (assembly != null)
                    {
                        connectionType = assembly.GetType("MySqlConnector.MySqlConnection");
                    }
                    else
                    {
                        assembly = LoadAssembly("MySql.Data");
                        connectionType = assembly.GetType("MySql.Data.MySqlClient.MySqlConnection");
                    }
                }
                if (connectionType == null)
                {
                    throw new NotImplementedException(provider.ToString());
                }
                var parameter = Expression.Parameter(typeof(string), "connectionString");
                var body = Expression.Convert(Expression.New(connectionType.GetConstructor(new Type[] { typeof(string) }), parameter), typeof(IDbConnection));
                var lambda = Expression.Lambda(body, parameter);
                return new ConnectionFactory((Func<string, IDbConnection>)lambda.Compile());
            });
        }

        private static Assembly LoadAssembly(string assemblyName)
        {
            try
            {
                return Assembly.Load(new AssemblyName(assemblyName));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
