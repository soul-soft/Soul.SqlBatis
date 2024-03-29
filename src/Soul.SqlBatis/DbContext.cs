﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis
{
    public abstract class DbContext : IDisposable
    {
        private IModel _model;

        public IModel Model => _model;

        private IDbConnection _connection;

        public DbContextOptions Options { get; }

        private IDbContextTransaction _currentTransaction;

        public IDbContextTransaction CurrentTransaction => _currentTransaction;

        private ChangeTracker _changeTracker;

        public ChangeTracker ChangeTracker => _changeTracker;

        public DbContext()
        {
            Options = BuildDbContextOptions();
            Initialize();
        }

        public DbContext(DbContextOptions options)
        {
            Options = options;
            Initialize();
        }

        private void Initialize()
        {
            _model = CreateModel(Options.ModelProvider);
            _changeTracker = new ChangeTracker(_model);
            _connection = Options.ConnectionFactory.Create();
        }

        private IModel CreateModel(IModelProvider modelProvider)
        {
            var builder = new ModelBuilder();
            return modelProvider.Create(GetType(), OnModelCreating);
        }

        private DbContextOptions BuildDbContextOptions()
        {
            var builder = new DbContextOptionsBuilder();
            OnConfiguring(builder);
            return builder.Build();
        }

        public DbSet<T> Set<T>()
            where T : class
        {
            return new DbSet<T>(this, new DynamicParameters());
        }

        public DbSet<T> Set<T>(DynamicParameters parameters)
           where T : class
        {
            return new DbSet<T>(this, parameters);
        }

        public DbSet<T> FromSql<T>(SqlBuilder whereBuilder, DynamicParameters parameters = null)
            where T : class
        {
            return new DbSet<T>(this, whereBuilder, parameters ?? new DynamicParameters());
        }

        public IDbQueryable<T> FromSql<T>(string fromSql)
          where T : class
        {
            return new DbSet<T>(this, fromSql, new DynamicParameters());
        }

        public IDbQueryable<T> FromSql<T>(string fromSql, DynamicParameters parameters = null)
            where T : class
        {
            return new DbSet<T>(this, fromSql, parameters ?? new DynamicParameters());
        }

        public IEntityEntry<T> Entry<T>(T entity)
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

        public void Attach(object entity)
        {
            Entry(entity).State = EntityState.Unchanged;
        }

        public void Attach<T>(T entity)
            where T : class
        {
            Entry(entity).State = EntityState.Unchanged;
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
            if (ChangeTracker.HasEntry(entity))
            {
                return;
            }
            Entry(entity).State = EntityState.Modified;
        }

        public void Update(object entity)
        {
            if (ChangeTracker.HasEntry(entity))
            {
                return;
            }
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

        public Task<int> SaveChangesAsync(CancellationToken? cancellationToken = default)
        {
            return new DbContextCommand(this).SaveChangesAsync(cancellationToken);
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

        public async Task OpenDbConnectionAsync(CancellationToken? cancellationToken = default)
        {
            var connection = (DbConnection)_connection;
            if (_connection.State == ConnectionState.Closed)
            {
                await (cancellationToken == null ? connection.OpenAsync() : connection.OpenAsync(cancellationToken.Value));
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
            var currentTransaction = new DbContextTransaction(transaction ?? _connection.BeginTransaction());
            void transactionEnd()
            {
                _currentTransaction = null;
                if (autoCloase)
                {
                    ColseDbConnection();
                }
            }
            currentTransaction.OnTransactionCommitEnd += transactionEnd;
            currentTransaction.OnTransactionRollbackEnd += transactionEnd;
            _currentTransaction = currentTransaction;
            return _currentTransaction;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(IDbTransaction transaction = null)
        {
            var autoCloase = false;
            if (_connection.State == ConnectionState.Closed)
            {
                await OpenDbConnectionAsync();
                autoCloase = true;
            }
            var currentTransaction = new DbContextTransaction(transaction ?? _connection.BeginTransaction());
            void transactionEndAction()
            {
                _currentTransaction = null;
                if (autoCloase)
                {
                    ColseDbConnection();
                }
            }
            currentTransaction.OnTransactionCommitEnd += transactionEndAction;
            currentTransaction.OnTransactionRollbackEnd += transactionEndAction;
            _currentTransaction = currentTransaction;
            return _currentTransaction;
        }

        protected virtual void OnModelCreating(ModelBuilder builder)
        {

        }

        protected virtual void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
       
        public virtual List<dynamic> Query(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return ExecuteDbCommandStrategy(() =>
            {
                Logging(sql, param);
                return _connection.Query(sql, param, GetDbTransaction(), commandTimeout, commandType);
            });
        }

        public virtual List<T> Query<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return ExecuteDbCommandStrategy(() =>
            {
                Logging(sql, param);
                return _connection.Query<T>(sql, param, GetDbTransaction(), commandTimeout, commandType);
            });
        }
        
        public virtual async Task<List<dynamic>> QueryAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken? cancellationToken = default)
        {
            return await ExecuteDbCommandStrategyAsync(async () =>
            {
                Logging(sql, param);
                return await _connection.QueryAsync(sql, param, GetDbTransaction(), commandTimeout, commandType, cancellationToken);
            });
        }

        public virtual async Task<List<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken? cancellationToken = default)
        {
            return await ExecuteDbCommandStrategyAsync(async () =>
            {
                Logging(sql, param);
                return await _connection.QueryAsync<T>(sql, param, GetDbTransaction(), commandTimeout, commandType, cancellationToken);
            });
        }

        public virtual int Execute(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return ExecuteDbCommandStrategy(() =>
            {
                Logging(sql, param);
                return _connection.Execute(sql, param, GetDbTransaction(), commandTimeout, commandType);
            });
        }

        public virtual Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken? cancellationToken = default)
        {
            return ExecuteDbCommandStrategyAsync(() =>
            {
                Logging(sql, param);
                return _connection.ExecuteAsync(sql, param, GetDbTransaction(), commandTimeout, commandType, cancellationToken);
            });
        }

        public virtual T ExecuteScalar<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return ExecuteDbCommandStrategy(() =>
            {
                Logging(sql, param);
                return _connection.ExecuteScalar<T>(sql, param, GetDbTransaction(), commandTimeout, commandType);
            });
        }

        public virtual Task<T> ExecuteScalarAsync<T>(string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken? cancellationToken = default)
        {
            return ExecuteDbCommandStrategyAsync(() =>
            {
                Logging(sql, param);
                return _connection.ExecuteScalarAsync<T>(sql, param, GetDbTransaction(), commandTimeout, commandType, cancellationToken);
            });
        }

        protected virtual void Logging(string sql, object param)
        {

        }

        private T ExecuteDbCommandStrategy<T>(Func<T> func)
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

        private async Task<T> ExecuteDbCommandStrategyAsync<T>(Func<Task<T>> func)
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
