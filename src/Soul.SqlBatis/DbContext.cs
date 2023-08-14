using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis
{
    public abstract class DbContext : IDisposable
    {
        private readonly Model _model;

        public Model Model => _model;

        private IDbConnection _connection;

        public DbContextOptions Options { get; }

        private IDbContextTransaction _currentTransaction;

        public IDbContextTransaction CurrentTransaction => _currentTransaction;

        private readonly ChangeTracker _changeTracker;

        public ChangeTracker ChangeTracker => _changeTracker;

        public DbContext(DbContextOptions options)
        {
            Options = options;
            _model = ModelCreating();
            _connection = options.ConnecionProvider();
            _changeTracker = new ChangeTracker(_model);
        }

        public DbSet<T> Set<T>()
            where T : class
        {
            return new DbSet<T>(this);
        }

        public IDbQueryable<T> FromSql<T>(string fromSql)
            where T : class
        {
            return new DbSet<T>(this).FromSql(fromSql);
        }

        public EntityEntry<T> Entry<T>(T entity)
            where T : class
        {
            return _changeTracker.TrackGraph(entity);
        }

        public T Find<T>(object key)
        {
            var entityEntry = ChangeTracker.Find(typeof(T), key);
            if (entityEntry != null)
            {
                return (T)entityEntry.Entity;
            }
            return new DbContextCommand(this).Find<T>(key);
        }

        public async Task<T> FindAsync<T>(object[] key)
        {
            var entityEntry = ChangeTracker.Find(typeof(T), key);
            if (entityEntry != null)
            {
                return (T)entityEntry.Entity;
            }
            return await new DbContextCommand(this).FindAsync<T>(key);
        }

        public void Add<T>(T entity)
            where T : class
        {
            Entry(entity).State = EntityState.Added;
        }

        public void Add(object entity)
        {
            Entry(entity).State = EntityState.Added;
        }

        public void AddRange<T>(IEnumerable<T> entities)
            where T : class
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }


        public void AddRange(IEnumerable<object> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public void Update<T>(T entity)
            where T : class
        {

            Entry(entity).State = EntityState.Modified;
        }

        public void Update(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        public void UpdateRange<T>(IEnumerable<T> entities)
            where T : class
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }

        public void UpdateRange(IEnumerable<object> entities)
        {
            foreach (var entity in entities)
            {
                Update(entity);
            }
        }

        public void Remove<T>(T entity)
            where T : class
        {
            Entry(entity).State = EntityState.Deleted;
        }

        public void Remove(object entity)
        {
            Entry(entity).State = EntityState.Deleted;
        }

        public void RemoveRange<T>(IEnumerable<T> entities)
            where T : class
        {
            foreach (var entity in entities)
            {
                Remove(entity);
            }
        }

        public void RemoveRange(IEnumerable<object> entities)
        {
            foreach (var entity in entities)
            {
                Remove(entity);
            }
        }

        public int SaveChanges()
        {
            return new DbContextCommand(this).SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return new DbContextCommand(this).SaveChangesAsync();
        }


        public IDbConnection GetDbConnection()
        {
            return _connection;
        }

        public IDbTransaction GetDbTransaction()
        {
            return CurrentTransaction?.DbTransaction;
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
                CurrentTransaction?.Dispose();
                _connection.Close();
            }
        }

        public Task ColseDbConnectionAsync()
        {
            ColseDbConnection();
            return Task.CompletedTask;
        }

        public IDbContextTransaction BeginTransaction(IDbTransaction transaction = null)
        {
            var autoCloase = false;
            if (_connection.State == ConnectionState.Closed)
            {
                OpenDbConnection();
                autoCloase = true;
            }
            transaction = transaction ?? _connection.BeginTransaction();
            _currentTransaction = new DbContextTransaction(transaction, () =>
            {
                _currentTransaction = null;
                if (autoCloase)
                {
                    ColseDbConnection();
                }
            });
            return CurrentTransaction;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(IDbTransaction transaction = null)
        {
            var autoCloase = false;
            if (_connection.State == ConnectionState.Closed)
            {
                await OpenDbConnectionAsync();
                autoCloase = true;
            }
            transaction = transaction ?? _connection.BeginTransaction();
            _currentTransaction = new DbContextTransaction(transaction, () =>
            {
                _currentTransaction = null;
                if (autoCloase)
                {
                    ColseDbConnection();
                }
            });
            return CurrentTransaction;
        }

        private Model ModelCreating()
        {
            return ModelBuilder.CreateDbContextModel(GetType(), OnModelCreating);
        }

        protected virtual void OnModelCreating(ModelBuilder builder)
        {

        }

        public virtual List<T> Query<T>(string sql, object param = null)
        {
            return DbCommandExecuteStrategy(() =>
            {
                Logging(sql);
                return _connection.Query<T>(sql, param, GetDbTransaction());
            });
        }

        public virtual async Task<List<T>> QueryAsync<T>(string sql, object param = null)
        {
            return await DbCommandExecuteStrategyAsync(async () =>
            {
                Logging(sql);
                return await _connection.QueryAsync<T>(sql, param, GetDbTransaction());
            });
        }

        public virtual int Execute(string sql, object param = null)
        {
            return DbCommandExecuteStrategy(() =>
            {
                Logging(sql);
                return _connection.Execute(sql, param, GetDbTransaction());
            });
        }

        public virtual Task<int> ExecuteAsync(string sql, object param = null)
        {
            return DbCommandExecuteStrategyAsync(() =>
            {
                Logging(sql);
                return _connection.ExecuteAsync(sql, param, GetDbTransaction());
            });
        }

        public virtual T ExecuteScalar<T>(string sql, object param = null)
        {
            return DbCommandExecuteStrategy(() =>
            {
                Logging(sql);
                return _connection.ExecuteScalar<T>(sql, param, GetDbTransaction());
            });
        }

        public virtual Task<T> ExecuteScalarAsync<T>(string sql, object param = null)
        {
            return DbCommandExecuteStrategyAsync(() =>
            {
                Logging(sql);
                return _connection.ExecuteScalarAsync<T>(sql, param, GetDbTransaction());
            });
        }

        protected virtual void Logging(string sql)
        {
            if (Options.LoggerFactory == null)
            {
                return;
            }
            var logger = Options.LoggerFactory.CreateLogger<DbContext>();
            logger.LogInformation(sql);
        }

        private T DbCommandExecuteStrategy<T>(Func<T> func)
        {
            var autoCloase = false;
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    OpenDbConnection();
                    autoCloase = true;
                }
                return func();
            }
            finally
            {
                if (autoCloase)
                {
                    ColseDbConnection();
                }
            }
        }

        private async Task<T> DbCommandExecuteStrategyAsync<T>(Func<Task<T>> func)
        {
            var autoCloase = false;
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await OpenDbConnectionAsync();
                    autoCloase = true;
                }
                return await func();
            }
            finally
            {
                if (autoCloase)
                {
                    await ColseDbConnectionAsync();
                }
            }
        }

        public void Dispose()
        {
            try
            {
                _currentTransaction?.Dispose();
            }
            finally
            {
                _currentTransaction = null;
                try
                {
                    _connection?.Close();
                    _connection?.Dispose();
                }
                finally
                {
                    _connection = null;
                }
            }
        }
    }
}
