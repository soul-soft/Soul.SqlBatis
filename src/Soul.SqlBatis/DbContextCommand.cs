using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var entries = _context.ChangeTracker.Entries()
                .Where(a => a.State == EntityState.Added || a.State == EntityState.Modified || a.State == EntityState.Deleted)
                .ToList();
            if (!entries.Any())
            {
                return row;
            }
            IDbContextTransaction transaction = null;
            var hasActiveDbTransaction = _context.CurrentTransaction != null;
            try
            {
                transaction = hasActiveDbTransaction ?
                    _context.CurrentTransaction
                    : _context.BeginTransaction();
                foreach (var entry in entries)
                {
                    if (entry.State == EntityState.Added)
                    {
                        row += ExecuteInsert(entry);
                    }
                    else if (entry.State == EntityState.Modified)
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

        public async Task<int> SaveChangesAsync()
        {
            var row = 0;
            var entries = _context.ChangeTracker.Entries()
                .Where(a => a.State == EntityState.Added || a.State == EntityState.Modified || a.State == EntityState.Deleted)
                .ToList();
            if (!entries.Any())
            {
                return row;
            }
            IDbContextTransaction transaction = null;
            var hasActiveDbTransaction = _context.CurrentTransaction != null;
            try
            {
                transaction = hasActiveDbTransaction ?
                    _context.CurrentTransaction
                    : await _context.BeginTransactionAsync();
                foreach (var entry in entries)
                {
                    if (entry.State == EntityState.Added)
                    {
                        row += await ExecuteInsertAsync(entry);
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        row += await ExecuteUpdateAsync(entry);
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        row += await ExecuteDeleteAsync(entry);
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
            values.Add(keyProperty.Property.Name, key);
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
            values.Add(keyProperty.Property.Name, key);
            var entity = (await _context.QueryAsync<T>(sql, values)).FirstOrDefault();
            if (entity != null)
            {
                _context.ChangeTracker.TrackGraph(entity);
            }
            return entity;
        }

        private int ExecuteInsert(EntityEntry entry)
        {
            var sql = BuildInsertSql(entry);
            var values = entry.Properties.ToDictionary(s => s.Property.Name, s => s.CurrentValue);
            if (!entry.Properties.Any(a => a.IsIdentity))
            {
                return _context.Execute(sql, values);
            }
            var id = _context.ExecuteScalar<long>(sql, values);
            SetIdentityPropertyValue(entry, id);
            return 1;
        }

        private async Task<int> ExecuteInsertAsync(EntityEntry entry)
        {
            var sql = BuildInsertSql(entry);
            var values = entry.Properties.ToDictionary(s => s.Property.Name, s => s.CurrentValue);
            if (!entry.Properties.Any(a => a.IsIdentity))
            {
                return await _context.ExecuteAsync(sql, values);
            }
            var id = await _context.ExecuteScalarAsync<long>(sql, values);
            SetIdentityPropertyValue(entry, id);
            return 1;
        }


        private int ExecuteUpdate(EntityEntry entityEntry)
        {
            var sql = BuildUpdateSql(entityEntry);
            var values = entityEntry.Properties
                .Where(a => !a.IsNotMapped)
                .Where(a => a.IsModified || a.IsKey)
                .ToDictionary(s => s.Property.Name, s => s.CurrentValue);
            return _context.Execute(sql, values);
        }

        private Task<int> ExecuteUpdateAsync(EntityEntry entityEntry)
        {
            var sql = BuildUpdateSql(entityEntry);
            var values = entityEntry.Properties
                .Where(a => !a.IsNotMapped)
                .Where(a => a.IsModified || a.IsKey)
                .ToDictionary(s => s.Property.Name, s => s.CurrentValue);
            return _context.ExecuteAsync(sql, values);
        }

        private int ExecuteDelete(EntityEntry entityEntry)
        {
            var sql = BuildDeleteSql(entityEntry);
            var values = entityEntry.Properties
                .Where(a => a.IsKey)
                .ToDictionary(s => s.Property.Name, s => s.OriginalValue);
            return _context.Execute(sql, values);
        }

        private Task<int> ExecuteDeleteAsync(EntityEntry entityEntry)
        {
            var sql = BuildDeleteSql(entityEntry);
            var values = entityEntry.Properties
                .Where(a => a.IsKey)
                .ToDictionary(s => s.Property.Name, s => s.OriginalValue);
            return _context.ExecuteAsync(sql, values);
        }

        private static string BuildInsertSql(EntityEntry entityEntry)
        {
            var properties = entityEntry.Properties
                .Where(a => !a.IsNotMapped)
                .Where(a => !a.IsIdentity);
            var columns = properties.Select(s => s.ColumnName);
            var parameters = properties.Select(s => $"@{s.Property.Name}");
            var sql = $"INSERT INTO {entityEntry.TableName} ({string.Join(",", columns)}) VALUES ({string.Join(",", parameters)})";
            if (entityEntry.Properties.Any(a => a.IsIdentity))
            {
                return $"{sql};SELECT LAST_INSERT_ID();";
            }
            return sql;
        }

        private static string BuildUpdateSql(EntityEntry entityEntry)
        {
            var columns = entityEntry.Properties
                .Where(a => !a.IsKey)
                .Where(a => !a.IsNotMapped)
                .Where(a => a.IsModified)
                .Select(s => $"{s.ColumnName} = @{s.Property.Name}");
            var wheres = BuildWhereSql(entityEntry);
            var sql = $"UPDATE {entityEntry.TableName} SET {string.Join(", ", columns)} WHERE {string.Join(" AND ", wheres)}";
            return sql;
        }

        private static string BuildDeleteSql(EntityEntry entityEntry)
        {
            var wheres = BuildWhereSql(entityEntry);
            var parameters = entityEntry.Properties
                .Where(a => a.IsKey)
                .Select(s => $"@{s.Property.Name}");
            var sql = $"DELETE FROM {entityEntry.TableName} WHERE {wheres}";
            return sql;
        }

        private static string BuildWhereSql(IEntityType entityEntry)
        {
            var keys = entityEntry.Properties
                .Where(a => a.IsKey);
            if (!keys.Any())
            {
                throw new ModelException(string.Format("Primary key not found in '{0}' type", entityEntry.Type.Name));
            }
            return string.Join(" AND ", keys.Select(s => $"{s.ColumnName} = @{s.Property.Name}"));
        }

        private static string BuildFindSql(IEntityType entityType)
        {
            var wheres = BuildWhereSql(entityType);
            var columns = entityType.Properties
                .Where(a => a.IsNotMapped)
                .Select(s =>
                {
                    if (s.ColumnName != s.Property.Name)
                    {
                        return $"{s.ColumnName} AS {s.Property.Name}";
                    }
                    return s.ColumnName;
                });
            return $"SELECT {string.Join(", ", columns)} FROM {entityType.TableName} WHERE {wheres}";
        }


    }
}
