using Soul.SqlBatis.ChangeTracking;
using Soul.SqlBatis.Infrastructure;
using Soul.SqlBatis.Metadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Soul.SqlBatis
{
    public class DbContext : IDisposable
    {
        private bool _disposed;

        private IModel _model;

        public IModel Model => _model;

        private SqlMapper _sql;

        public SqlMapper Sql => _sql;

        private IDbConnection _connection;

        public DbContextTransaction CurrentTransaction { get; private set; }

        private readonly IChangeTracker _changeTracker;

        public IChangeTracker ChangeTracker => _changeTracker;

        private DbContextOptions _options;

        public DbContextOptions Options => _options;

        internal void WriteLog(string sql, object param)
        {
            Options.Loggger?.Invoke(sql, param);
        }

        public DbContext(Action<DbContextOptions> configureOptions)
        {
            _options = new DbContextOptions();
            configureOptions(_options);
            var settings = GetSettings();
            _connection = _options.Connection;
            _sql = new SqlMapper(this, settings);
            _model = new Model(settings);
            _changeTracker = new ChangeTracker(this);
        }

        internal SqlSettings GetSettings()
        {
            if (Options.DbType == DbType.MySql)
            {
                return DbContextSettings.MySql;
            }
            else if (Options.DbType == DbType.Npgsql)
            {
                return DbContextSettings.Npgsql;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public virtual SqlBuilder CreateSqlBuilder()
        {
            return new SqlBuilder(GetSettings());
        }

        public virtual EntityEntry Attach(object entity)
        {
            var entry = ChangeTracker.Entry(entity);
            entry.State = EntityState.Unchanged;
            return entry;
        }

        public virtual EntityEntry<T> Entry<T>(T entity)
        {
            var entry = ChangeTracker.Entry(entity);
            return new EntityEntry<T>(entry);
        }

        public virtual void Add<T>(T entity) where T : class
        {
            Entry(entity).State = EntityState.Added;
        }

        public virtual void AddRange<T>(IEnumerable<T> entities) where T : class
        {
            foreach (var item in entities)
            {
                Add(item);
            }
        }

        public virtual void Update<T>(T entity) where T : class
        {
            var entry = Entry(entity);
            entry.State = EntityState.Modified;
            return;
        }

        public virtual void UpdateRange<T>(IEnumerable<T> entities, bool ignoreNullMembers = false) where T : class
        {
            foreach (var item in entities)
            {
                Update(item);
            }
        }

        public virtual void Remove<T>(T entity) where T : class
        {
            Entry(entity).State = EntityState.Deleted;
        }

        public virtual void RemoveRange<T>(IEnumerable<T> entities) where T : class
        {
            foreach (var item in entities)
            {
                Remove(item);
            }
        }

        public virtual DbSet<T> Set<T>() where T : class
        {
            return new DbSet<T>(this, new DynamicParameters());
        }

        public virtual DbSet<T> Set<T>(DynamicParameters parameters) where T : class
        {
            return new DbSet<T>(this, parameters);
        }

        public IDbConnection GetDbConnection()
        {
            return _connection;
        }

        public virtual bool OpenConnection()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
                return true;
            }
            return false;
        }

        public virtual async Task<bool> OpenConnectionAsync()
        {
            if (_connection.State != ConnectionState.Open)
            {
                await (_connection as DbConnection).OpenAsync();
                return true;
            }
            return false;
        }

        public virtual void CloseConnection()
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        public virtual bool HasTransaction()
        {
            return CurrentTransaction != null;
        }


        public virtual DbContextTransaction BeginTransaction()
        {
            if (CurrentTransaction != null)
            {
                throw new NotSupportedException("A transaction is already in progress. Nested transactions are not supported.");
            }
            var closeConnection = OpenConnection();
            CurrentTransaction = new DbContextTransaction(_connection.BeginTransaction(), () =>
            {
                CurrentTransaction = null;
                if (closeConnection)
                    CloseConnection();
            });
            return CurrentTransaction;
        }

        public virtual async Task<DbContextTransaction> BeginTransactionAsync()
        {
            if (CurrentTransaction != null)
            {
                throw new NotSupportedException("A transaction is already in progress. Nested transactions are not supported.");
            }
            var closeConnection = await OpenConnectionAsync();
            CurrentTransaction = new DbContextTransaction(_connection.BeginTransaction(), () =>
            {
                CurrentTransaction = null;
                if (closeConnection)
                    CloseConnection();
            });
            return CurrentTransaction;
        }
       
        public virtual int SaveChanges()
        {
            var command = new DbContextCommand(this, GetSettings());
            if (HasTransaction())
            {
                return command.SaveChanges();
            }
            else
            {
                using (var transaction = BeginTransaction())
                {
                    var row = command.SaveChanges();
                    transaction.CommitTransaction();
                    return row;
                }
            }
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            var command = new DbContextCommand(this, GetSettings());
            if (HasTransaction())
            {
                return await command.SaveChangesAsync();
            }
            else
            {
                using (var transaction = BeginTransaction())
                {
                    var row = command.SaveChanges();
                    transaction.CommitTransaction();
                    return row;
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    CurrentTransaction?.Dispose();
                }
                catch { }
                try
                {
                    if (_connection.State != ConnectionState.Closed)
                    {
                        _connection.Close();
                    }
                    _connection?.Dispose();
                }
                catch { }
                _disposed = true;
            }
        }
    }
}
