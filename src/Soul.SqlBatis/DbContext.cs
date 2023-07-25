using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Soul.SqlBatis.Linq;
using Soul.SqlBatis.Model;

namespace Soul.SqlBatis
{
    public abstract class DbContext : IDisposable
    {
        private IModel _model;
        
        private IDbConnection _connection;
        
        private DbContextTransaction _transaction;
        
        private DbContextOptions _options;
        
        public IModel Model => _model;
       
        public DbContextOptions Options => _options;
       
        public DbContextTransaction CurrentDbTransaction => _transaction;

        public DbContext()
        {
            var modelBuilder = CreateModelBuilder();
            OnModelCreating(modelBuilder);
            _model = modelBuilder.Build();
        }

        public DbSet<T> Set<T>()
            where T : class
        {
            return new DbSet<T>(this);
        }

        public IDbConnection GetDbConnection()
        {
            return _connection;
        }

        public void OpenDbConnection()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        public async Task OpenDbConnectionAsync()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                if (_connection is DbConnection connection)
                {
                    await connection.OpenAsync();
                }
                else
                {
                    _connection.Open();
                }
            }
        }

        public void ColseDbConnection()
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        public Task ColseDbConnectionAsync()
        {
            ColseDbConnection();
            return Task.CompletedTask;
        }

        public DbContextTransaction BeginTransaction()
        {
            var transaction = _connection.BeginTransaction();
            return new DbContextTransaction(transaction);
        }

        public Task<DbContextTransaction> BeginTransactionAsync()
        {
            var transaction = _connection.BeginTransaction();
            return Task.FromResult(new DbContextTransaction(transaction));
        }

        protected void OnModelCreating(ModelBuilder builder)
        {
            
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
        }

        private ModelBuilder CreateModelBuilder()
        {
            var builder = new ModelBuilder();
            var entities = GetType().GetProperties()
                .Where(a=>a.PropertyType.IsGenericType)
                .Where(a=>a.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(s => s.PropertyType.GenericTypeArguments[0]);
            return builder;
        }
    }
}
