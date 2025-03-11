using Soul.SqlBatis.ChangeTracking;
using Soul.SqlBatis.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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

        private DbContextTransaction _transaction;

        private readonly DbContextUnitWork _unitWork;

        public DbContextTransaction CurrentTransaction => _transaction;

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
            _model = new AnnotationModel(settings);
            _unitWork = new DbContextUnitWork(this, settings);
            _changeTracker = new ChangeTracker(_model);
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

        public virtual EntityEntry<T> Attach<T>(T entity)
        {
            var entry = ChangeTracker.Track(entity);
            entry.State = EntityState.Unchanged;
            return entry;
        }

        public virtual IEnumerable<EntityEntry<T>> AttachRange<T>(IEnumerable<T> entities)
        {
            return entities.Select(s => Attach(s));
        }

        public virtual void Detach<T>(T entity)
        {
            ChangeTracker.Untrack(entity);
        }

        public virtual EntityEntry<T> Entry<T>(T entity)
        {
            return ChangeTracker.Track(entity);
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

        public virtual void Update<T>(T entity, bool ignoreNullMembers = false) where T : class
        {
            var entry = ChangeTracker.Track(entity, ignoreNullMembers);
            entry.State = EntityState.Modified;
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

        public IDbTransaction GetDbTransaction()
        {
            return _transaction?.DbTransaction;
        }


        public virtual DbContextTransaction BeginTransaction()
        {
            var closeConnection = false;
            if (_transaction == null)
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                    closeConnection = true;
                }
                _transaction = new DbContextTransaction(_connection.BeginTransaction(), () =>
                {
                    _transaction = null;
                    if (closeConnection)
                        _connection.Close();
                });
                return _transaction;
            }
            else
            {
                return _transaction;
            }
        }

        public virtual int SaveChanges()
        {
            var transaction = CurrentTransaction;
            var isTransactionOwner = CurrentTransaction == null;
            try
            {
                if (transaction == null)
                {
                    transaction = BeginTransaction();
                }
                var affectedRows = _unitWork.SaveChanges(ChangeTracker.GetChangedEntries());
                if (isTransactionOwner)
                {
                    transaction.CommitTransaction();
                }
                ChangeTracker.ClearEntities();
                return affectedRows;
            }
            finally
            {
                if (isTransactionOwner)
                {
                    transaction?.Dispose();
                }
            }
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            var transaction = CurrentTransaction;
            var isTransactionOwner = CurrentTransaction == null;
            try
            {
                if (transaction == null)
                {
                    transaction = BeginTransaction();
                }
                var affectedRows = await _unitWork.SaveChangesAsync(ChangeTracker.GetChangedEntries());
                if (isTransactionOwner)
                {
                    transaction.CommitTransaction();
                }
                ChangeTracker.ClearEntities();
                return affectedRows;
            }
            finally
            {
                if (isTransactionOwner)
                {
                    transaction?.Dispose();
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    _transaction?.Dispose();
                    _transaction = null;
                }
                catch { }
                try
                {
                    if (_connection.State != ConnectionState.Closed)
                    {
                        _connection?.Close();
                    }
                    _connection?.Dispose();
                    _connection = null;
                }
                catch { }
                _disposed = true;
            }
        }
    }
}
