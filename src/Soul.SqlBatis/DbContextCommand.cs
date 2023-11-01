using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Soul.SqlBatis.Exceptions;

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
            var entries = GetCacheEntries();
            if (!HasChangeEntries(entries))
            {
                return row;
            }
            IDbContextTransaction transaction = null;
            var hasActiveDbTransaction = _context.CurrentTransaction != null;
            try
            {
                transaction = hasActiveDbTransaction ? _context.CurrentTransaction : _context.BeginTransaction();
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

        public async Task<int> SaveChangesAsync(CancellationToken? cancellationToken = default)
        {
            var row = 0;
            var entries = GetCacheEntries();
            if (!HasChangeEntries(entries))
            {
                return row;
            }
            IDbContextTransaction transaction = null;
            var hasActiveDbTransaction = _context.CurrentTransaction != null;
            try
            {
                transaction = hasActiveDbTransaction ? _context.CurrentTransaction : await _context.BeginTransactionAsync();
                foreach (var entry in entries)
                {
                    if (entry.State == EntityState.Added)
                    {
                        row += await ExecuteInsertAsync(entry, cancellationToken);
                    }
                    else if (entry.State == EntityState.Modified)
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

        private List<IEntityEntry> GetCacheEntries()
        {
            var entries = _context.ChangeTracker.Entries();
            return entries.Select(s =>
            {
                var values = s.Values.Select(v =>
                {
                    var currentValue = v.CurrentValue;
                    var originalValue = v.OriginalValue;
                    var isModified = (v as EntityPropertyEntry).CalcModified(currentValue, originalValue);
                    return new EntityPropertyEntryCache(v, currentValue, originalValue, isModified);
                }).ToList();
                var entry = s as EntityEntry;
                var state = entry.CalcState(values);
                return new EntityEntryCache(s, s.Entity, values, state);
            })
            .Cast<IEntityEntry>()
            .ToList();
        }

        public bool HasChangeEntries(List<IEntityEntry> entries)
        {
            return entries.Where(a => a.State == EntityState.Added || a.State == EntityState.Modified || a.State == EntityState.Deleted || a.Values.Any(p => p.IsModified)).Any();
        }

        public T Find<T>(object key)
        {
            var entityType = _context.Model.GetEntityType(typeof(T));
            var sql = BuildFindCommand(entityType);
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
            var sql = BuildFindCommand(entityType);
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

        private int ExecuteInsert(IEntityEntry entry)
        {
            var (sql, values) = BuildInsertCommand(entry);
            if (!entry.Values.Any(a => a.IsIdentity))
            {
                return _context.Execute(sql, values);
            }
            var id = _context.ExecuteScalar<long>(sql, values);
            SetIdentityPropertyValue(entry, id);
            return 1;
        }

        private async Task<int> ExecuteInsertAsync(IEntityEntry entry, CancellationToken? cancellationToken = default)
        {
            var (sql, values) = BuildInsertCommand(entry);
            if (!entry.Values.Any(a => a.IsIdentity))
            {
                return await _context.ExecuteAsync(sql, values, cancellationToken: cancellationToken);
            }
            var id = await _context.ExecuteScalarAsync<long>(sql, values);
            SetIdentityPropertyValue(entry, id);
            return 1;
        }

        private static void SetIdentityPropertyValue(IEntityEntry entity, object identity)
        {
            var property = entity.Values.Where(a => a.IsIdentity).First().Property;
            var type = Nullable.GetUnderlyingType(property.PropertyType)
                ?? property.PropertyType;
            property.SetValue(entity.Entity, Convert.ChangeType(identity, type));
        }

        private int ExecuteUpdate(IEntityEntry entity)
        {
            var (sql, values) = BuildUpdateCommand(entity);
            var row = _context.Execute(sql, values);
            if (row == 0)
            {
                throw new DbUpdateConcurrencyException("The data version is too old", entity);
            }
            return row;
        }

        private async Task<int> ExecuteUpdateAsync(IEntityEntry entity, CancellationToken? cancellationToken = default)
        {
            var (sql, values) = BuildUpdateCommand(entity);
            var row = await _context.ExecuteAsync(sql, values, cancellationToken: cancellationToken);
            if (row == 0)
            {
                throw new DbUpdateConcurrencyException("The data version is too old", entity);
            }
            return row;
        }

        private int ExecuteDelete(IEntityEntry entity)
        {
            var (sql, values) = BuildDeleteCommand(entity);
            return _context.Execute(sql, values);
        }

        private Task<int> ExecuteDeleteAsync(IEntityEntry entity, CancellationToken? cancellationToken = default)
        {
            var (sql, values) = BuildDeleteCommand(entity);
            return _context.ExecuteAsync(sql, values, cancellationToken: cancellationToken);
        }

        private static (string, object) BuildInsertCommand(IEntityEntry entity)
        {
            var func = TypeSerializer.CreateDeserializer(entity.Type);
            var properties = entity.Values
                .Where(a => !a.IsNotMapped)
                .Where(a => !a.IsIdentity);
            var columns = properties.Select(s => s.ColumnName);
            var values = properties.ToDictionary(s => s.CSharpName, s => s.CurrentValue);
            var parameters = properties.Select(s => $"@{s.CSharpName}");
            var sql = $"INSERT INTO {entity.TableName} ({string.Join(",", columns)}) VALUES ({string.Join(",", parameters)})";
            if (entity.Values.Any(a => a.IsIdentity))
            {
                return ($"{sql};SELECT LAST_INSERT_ID();", values);
            }
            return (sql, values);
        }

        private static (string, object) BuildUpdateCommand(IEntityEntry entity)
        {
            var properties = entity.Values
                    .Where(a => !a.IsNotMapped)
                    .Where(a => a.IsKey || a.IsConcurrencyToken || a.IsModified);
            var columns = properties.Where(a => !a.IsKey).Select(s => $"{s.ColumnName} = @{s.Property.Name}");
            var values = properties.ToDictionary(s => s.CSharpName, s => s.CurrentValue);
            foreach (var item in properties.Where(a => a.IsConcurrencyToken))
            {
                values.Add($"Old{item.CSharpName}", item.OriginalValue);
            }
            var wheres = BuildWhereCommand(entity);
            var sql = $"UPDATE {entity.TableName} SET {string.Join(", ", columns)} WHERE {string.Join(" AND ", wheres)}";
            return (sql, values);
        }

        private static (string, object) BuildDeleteCommand(IEntityEntry entity)
        {
            var wheres = BuildWhereCommand(entity);
            var properties = entity.Values
                .Where(a => a.IsKey);
            var values = properties
                .ToDictionary(s => s.Property.Name, s => s.OriginalValue);
            var sql = $"DELETE FROM {entity.TableName} WHERE {wheres}";
            return (sql, values);
        }

        private static string BuildWhereCommand(IEntityType entity)
        {
            var keys = entity.Properties
                .Where(a => a.IsKey || a.IsConcurrencyToken)
                .OrderByDescending(a => a.IsKey);
            if (!keys.Any())
            {
                throw new ModelException(string.Format("Primary key not found in '{0}' type", entity.Type.Name));
            }
            return string.Join(" AND ", keys.Select(s => $"{s.ColumnName} = @{(s.IsConcurrencyToken ? "Old" + s.CSharpName : s.CSharpName)}"));
        }

        private static string BuildFindCommand(IEntityType entityType)
        {
            var wheres = BuildWhereCommand(entityType);
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
