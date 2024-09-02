using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Soul.SqlBatis.ChangeTracking;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
    public class DbContext : IDisposable
    {
        private bool _disposed;

        public IModel Model => Options.Model;

        public IDatabaseCommand Command { get; private set; }

        internal IDbConnection Connection => Options.Connection;

        private IDbTransaction _transaction;

        public IDbTransaction CurrentTransaction => _transaction;

        public IEntityPersister EntityPersister => Options.EntityPersister;

        public IEntityMapper EntityMapper => Options.EntityMapper;

        public IChangeTracker ChangeTracker { get; private set; }

        public DbContextOptions Options { get; private set; } = new DbContextOptions();

        private void Init(Action<DbContextOptions> configure)
        {
            configure(Options);
            Command = new DatabaseCommand(this);
            ChangeTracker = new ChangeTracker(Options.Model);
        }

        internal void WriteLog(string sql, object param)
        {
            Options.Loggger?.Invoke(sql, param);
        }
        
        protected DbContext()
        {
        }

        public DbContext(Action<DbContextOptions> configure)
        {
            Init(configure);
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

        public virtual DbContextTransaction BeginTransaction()
        {
            var closeConnection = false;
            if (_transaction == null)
            {
                if (Connection.State == ConnectionState.Closed)
                {
                    Connection.Open();
                    closeConnection = true;
                }
                _transaction = Connection.BeginTransaction();
            }
            return new DbContextTransaction(_transaction, () =>
            {
                _transaction = null;
                if (closeConnection)
                    Connection.Close();
            });
        }

        public virtual int SaveChanges()
        {
            using (var transaction = BeginTransaction())
            {
                var affectedRows = EntityPersister.SaveChanges(Command, ChangeTracker.GetChangedEntries());
                transaction.CommitTransaction();
                ChangeTracker.ClearEntities();
                return affectedRows;
            }
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            using (var transaction = BeginTransaction())
            {
                var affectedRows = await EntityPersister.SaveChangesAsync(Command, ChangeTracker.GetChangedEntries());
                transaction.CommitTransaction();
                ChangeTracker.ClearEntities();
                return affectedRows;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Options = null;
                try
                {
                    _transaction?.Dispose();
                    _transaction = null;
                }
                catch { }
                try
                {
                    Connection?.Close();
                    Connection?.Dispose();
                }
                catch { }
                _disposed = true;
            }
        }
    }
}
