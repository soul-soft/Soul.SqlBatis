using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Soul.SqlBatis.Infrastructure
{
    internal class DbContextCommand
    {
        private readonly DbContext _context;

        public DbContextCommand(DbContext context)
        {
            _context = context;
        }

        public int SaveChanges()
        {
            var row = 0;
            if (!HasChanges())
            {
                return row;
            }
            IDbContextTransaction transaction = null;
            var hasActiveDbTransaction = _context.CurrentTransaction != null;
            try
            {
                transaction = hasActiveDbTransaction ? _context.CurrentTransaction : _context.BeginTransaction();
                var entries = _context.ChangeTracker.Entries();
                foreach (var entry in entries)
                {
                    if (entry.State == EntityState.Added)
                    {
                        row += ExecuteInsert(entry);
                    }
                    else if (entry.State == EntityState.Modified || entry.State == EntityState.Unchanged)
                    {
                        row += ExecuteUpdate(entry);
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        row += ExecuteDelete(entry);
                    }
                }
                if (!hasActiveDbTransaction)
                {
                    transaction.CommitTransaction();
                }
                _context.ChangeTracker.Clear();
            }
            finally
            {
                if (!hasActiveDbTransaction)
                {
                    transaction.Dispose();
                }
            }
            return row;
        }

        public async Task<int> SaveChangesAsync(CancellationToken? cancellationToken = default)
        {
            var row = 0;
            if (!HasChanges())
            {
                return row;
            }
            IDbContextTransaction transaction = null;
            var hasActiveDbTransaction = _context.CurrentTransaction != null;
            try
            {
                transaction = hasActiveDbTransaction ? _context.CurrentTransaction : await _context.BeginTransactionAsync();
                var entries = _context.ChangeTracker.Entries();
                foreach (var entry in entries)
                {
                    if (entry.State == EntityState.Added)
                    {
                        row += await ExecuteInsertAsync(entry, cancellationToken);
                    }
                    else if (entry.State == EntityState.Modified || entry.State == EntityState.Unchanged)
                    {
                        row += await ExecuteUpdateAsync(entry, cancellationToken);
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        row += await ExecuteDeleteAsync(entry, cancellationToken);
                    }
                }
                if (!hasActiveDbTransaction)
                {
                    await transaction.CommitTransactionAsync();
                }
                _context.ChangeTracker.Clear();
            }
            finally
            {
                if (!hasActiveDbTransaction)
                {
                    transaction?.Dispose();
                }
            }
            return row;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.Entries()
               .Where(a => a.State == EntityState.Added || a.State == EntityState.Modified || a.State == EntityState.Deleted || a.Properties.Any(p => p.IsModified))
               .Any();
        }

        public static void SetIdentityPropertyValue(EntityEntry entityEntry, object identity)
        {
            var property = entityEntry.Properties.Where(a => a.IsIdentity).First().Property;
            var type = Nullable.GetUnderlyingType(property.PropertyType)
                ?? property.PropertyType;
            property.SetValue(entityEntry.Entity, Convert.ChangeType(identity, type));
        }

        public T Find<T>(object key)
        {
            var entityType = _context.Model.GetEntityType(typeof(T));
            var sql = BuildFindSql(entityType);
            var values = new Dictionary<string, object>();
            var keyProperty = entityType.Properties.Where(a => a.IsKey).First();
            values.Add(keyProperty.CSharpName, key);
            var entity = _context.Query<T>(sql, values).FirstOrDefault();
            if (entity != null)
            {
                _context.ChangeTracker.TrackGraph(entity);
            }
            return entity;
        }

        public async Task<T> FindAsync<T>(object key)
        {
            var entityType = _context.Model.GetEntityType(typeof(T));
            var sql = BuildFindSql(entityType);
            var values = new Dictionary<string, object>();
            var keyProperty = entityType.Properties.Where(a => a.IsKey).First();
            values.Add(keyProperty.CSharpName, key);
            var entity = (await _context.QueryAsync<T>(sql, values)).FirstOrDefault();
            if (entity != null)
            {
                _context.ChangeTracker.TrackGraph(entity);
            }
            return entity;
        }

        private int ExecuteInsert(EntityEntry entry)
        {
            var (sql, values) = BuildInsertSql(entry);
            if (!entry.Properties.Any(a => a.IsIdentity))
            {
                return _context.Execute(sql, values);
            }
            var id = _context.ExecuteScalar<long>(sql, values);
            SetIdentityPropertyValue(entry, id);
            return 1;
        }

        private async Task<int> ExecuteInsertAsync(EntityEntry entry, CancellationToken? cancellationToken = default)
        {
            var (sql, values) = BuildInsertSql(entry);
            if (!entry.Properties.Any(a => a.IsIdentity))
            {
                return await _context.ExecuteAsync(sql, values, cancellationToken: cancellationToken);
            }
            var id = await _context.ExecuteScalarAsync<long>(sql, values);
            SetIdentityPropertyValue(entry, id);
            return 1;
        }

        private int ExecuteUpdate(EntityEntry entityEntry)
        {
            var (sql, values) = BuildUpdateSql(entityEntry);
            return _context.Execute(sql, values);
        }

        private Task<int> ExecuteUpdateAsync(EntityEntry entityEntry, CancellationToken? cancellationToken = default)
        {
            var (sql, values) = BuildUpdateSql(entityEntry);
            return _context.ExecuteAsync(sql, values, cancellationToken: cancellationToken);
        }

        private int ExecuteDelete(EntityEntry entityEntry)
        {
            var (sql, values) = BuildDeleteSql(entityEntry);
            return _context.Execute(sql, values);
        }

        private Task<int> ExecuteDeleteAsync(EntityEntry entityEntry, CancellationToken? cancellationToken = default)
        {
            var (sql, values) = BuildDeleteSql(entityEntry);
            return _context.ExecuteAsync(sql, values, cancellationToken: cancellationToken);
        }

        private static (string, object) BuildInsertSql(EntityEntry entityEntry)
        {
            var func = TypeSerializer.CreateDeserializer(entityEntry.Type);
            var properties = entityEntry.Properties
                .Where(a => !a.IsNotMapped)
                .Where(a => !a.IsIdentity);
            var columns = properties.Select(s => s.ColumnName);
            var values = func(entityEntry.Entity);
            values = values.Where(a => properties.Any(p => p.CSharpName == a.Key)).ToDictionary(s => s.Key, s => s.Value);
            var parameters = properties.Select(s => $"@{s.CSharpName}");
            var sql = $"INSERT INTO {entityEntry.TableName} ({string.Join(",", columns)}) VALUES ({string.Join(",", parameters)})";
            if (entityEntry.Properties.Any(a => a.IsIdentity))
            {
                return ($"{sql};SELECT LAST_INSERT_ID();", values);
            }
            return (sql, values);
        }

        private static (string, object) BuildUpdateSql(EntityEntry entityEntry)
        {
            if (entityEntry.State == EntityState.Modified)
            {
                var properties = entityEntry.Properties
                    .Where(a => !a.IsNotMapped);
                var columns = properties.Where(a => !a.IsKey).Select(s => $"{s.ColumnName} = @{s.Property.Name}");
                var values = properties.ToDictionary(s => s.CSharpName, s => s.CurrentValueCache);
                var wheres = BuildWhereSql(entityEntry);
                var sql = $"UPDATE {entityEntry.TableName} SET {string.Join(", ", columns)} WHERE {string.Join(" AND ", wheres)}";
                return (sql, values);
            }
            else
            {
                var properties = entityEntry.Properties
                    .Where(a => !a.IsNotMapped)
                    .Where(a => a.IsKey || a.IsModified);
                var columns = properties.Where(a => !a.IsKey).Select(s => $"{s.ColumnName} = @{s.Property.Name}");
                var values = properties.ToDictionary(s => s.CSharpName, s => s.CurrentValueCache);
                var wheres = BuildWhereSql(entityEntry);
                var sql = $"UPDATE {entityEntry.TableName} SET {string.Join(", ", columns)} WHERE {string.Join(" AND ", wheres)}";
                return (sql, values);
            }
        }

        private static (string, object) BuildDeleteSql(EntityEntry entityEntry)
        {
            var wheres = BuildWhereSql(entityEntry);
            var properties = entityEntry.Properties
                .Where(a => a.IsKey);
            var values = properties
                .ToDictionary(s => s.Property.Name, s => s.OriginalValue);
            var sql = $"DELETE FROM {entityEntry.TableName} WHERE {wheres}";
            return (sql, values);
        }

        private static string BuildWhereSql(IEntityType entityEntry)
        {
            var keys = entityEntry.Properties
                .Where(a => a.IsKey);
            if (!keys.Any())
            {
                throw new ModelException(string.Format("Primary key not found in '{0}' type", entityEntry.Type.Name));
            }
            return string.Join(" AND ", keys.Select(s => $"{s.ColumnName} = @{s.CSharpName}"));
        }

        private static string BuildFindSql(IEntityType entityType)
        {
            var wheres = BuildWhereSql(entityType);
            var columns = entityType.Properties
                .Where(a => !a.IsNotMapped)
                .Select(s =>
                {
                    if (s.ColumnName != s.CSharpName)
                    {
                        return $"{s.ColumnName} AS {s.CSharpName}";
                    }
                    return s.ColumnName;
                });
            return $"SELECT {string.Join(", ", columns)} FROM {entityType.TableName} WHERE {wheres}";
        }
    }
}
