using Soul.SqlBatis.ChangeTracking;
using Soul.SqlBatis.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soul.SqlBatis.Infrastructure
{
    internal class DbContextCommand
    {
        private readonly DbContext _context;
        private readonly SqlSettings _sqlSettings;

        public DbContextCommand(DbContext context, SqlSettings sqlSettings)
        {
            _context = context;
            _sqlSettings = sqlSettings;
        }

        public int SaveChanges()
        {
            var affectedRows = 0;
            var entityEntries = _context.ChangeTracker.Entities();
            foreach (var entityEntry in entityEntries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    affectedRows += Insert(entityEntry);
                    entityEntry.State = EntityState.Unchanged;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    affectedRows += Update(entityEntry);
                    entityEntry.State = EntityState.Unchanged;
                }
                else if (entityEntry.State == EntityState.Deleted)
                {
                    affectedRows += Delete(entityEntry);
                    entityEntry.State = EntityState.Detached;
                }
            }
            return affectedRows;
        }

        public async Task<int> SaveChangesAsync()
        {
            var affectedRows = 0;
            var entityEntries = _context.ChangeTracker.Entities();
            foreach (var entityEntry in entityEntries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    affectedRows += await InsertAsync(entityEntry);
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    affectedRows += await UpdateAsync(entityEntry);
                }
                else if (entityEntry.State == EntityState.Deleted)
                {
                    affectedRows += await DeleteAsync(entityEntry);
                }
            }
            return affectedRows;
        }

        private int Insert(IEntityEntry entityEntry)
        {
            var (sql, param) = BuildInsertCommand(entityEntry);
            var identityMember = entityEntry.Properties.FirstOrDefault(a => a.Metadata.ValueGenerated == ValueGenerated.OnAdd);
            if (identityMember != null)
            {
                var obj = _context.Sql.ExecuteScalar(sql, param);
                entityEntry.SetCurrentValue(identityMember.Metadata, obj);
                return 1;
            }
            else
            {
                return _context.Sql.Execute(sql, param);
            }
        }

        private int Update(IEntityEntry entityEntry)
        {
            var (sql, param) = BuildUpdateCommand(entityEntry);
            return _context.Sql.Execute(sql, param);
        }

        private int Delete(IEntityEntry entityEntry)
        {
            var (sql, param) = BuildDeleteCommand(entityEntry);
            return _context.Sql.Execute(sql, param);
        }

        private async Task<int> InsertAsync(IEntityEntry entityEntry)
        {
            var (sql, param) = BuildInsertCommand(entityEntry);
            var identityMember = entityEntry.Properties.FirstOrDefault(a => a.Metadata.ValueGenerated == ValueGenerated.OnAdd);
            if (identityMember != null)
            {
                var obj = await _context.Sql.ExecuteScalarAsync(sql, param);
                entityEntry.SetCurrentValue(identityMember.Metadata, obj);
                return 1;
            }
            else
            {
                return await _context.Sql.ExecuteAsync(sql, param);
            }
        }

        private Task<int> UpdateAsync(IEntityEntry entry)
        {
            var (sql, param) = BuildUpdateCommand(entry);
            return _context.Sql.ExecuteAsync(sql, param);
        }

        private Task<int> DeleteAsync(IEntityEntry entry)
        {
            var (sql, param) = BuildDeleteCommand(entry);
            return _context.Sql.ExecuteAsync(sql, param);
        }

        private (string, DynamicParameters) BuildInsertCommand(IEntityEntry entityEntry)
        {
            var members = entityEntry.Metadata.GetProperties()
                .Where(a => a.ValueGenerated != ValueGenerated.OnAdd)
                .ToList();
            var columns = string.Join(",", members.Select(s => s.GetColumnName()));
            var paramNames = string.Join(",", members.Select(s => $"@{s.Name}"));
            var sql = $"INSERT INTO {entityEntry.Metadata.GetTableName()} ({columns}) VALUES ({paramNames})";
            var identityMember = entityEntry.Properties.FirstOrDefault(a => a.Metadata.ValueGenerated == ValueGenerated.OnAdd);
            if (identityMember != null)
            {
                sql += $"{string.Format(_sqlSettings.IdentitySql, identityMember.Metadata.GetColumnName())}";
            }
            var param = new DynamicParameters();
            foreach (var item in entityEntry.Properties)
            {
                param.Add(item.Metadata.Name, item.CurrentValue);
            }
            return (sql, param);
        }

        private (string, DynamicParameters) BuildUpdateCommand(IEntityEntry entityEntry)
        {
            var members = entityEntry.Properties.Where(a => a.IsModified);
            var columns = string.Join(",", members.Select(s => $"{s.Metadata.GetColumnName()} = @{s.Metadata.Name}"));
            var primaryKey = entityEntry.Metadata.PrimaryKey;
            var whereSql = string.Join(" AND ", primaryKey.Properties.Select(s => $"{s.GetColumnName()} = @{s.Name}"));
            var sql = $"UPDATE {entityEntry.Metadata.GetTableName()} SET {columns} WHERE {whereSql}";
            var param = new DynamicParameters();
            foreach (var item in entityEntry.Properties)
            {
                param.Add(item.Metadata.Name, item.CurrentValue);
            }
            return (sql, param);
        }

        private (string, DynamicParameters) BuildDeleteCommand(IEntityEntry entityEntry)
        {
            var primaryKey = entityEntry.Metadata.PrimaryKey;
            var whereSql = string.Join(" AND ", primaryKey.Properties.Select(s => $"{s.GetColumnName()} = @{s.Name}"));
            var sql = $"DELETE FROM {entityEntry.Metadata.GetTableName()} WHERE {whereSql}";
            var param = new DynamicParameters();
            foreach (var item in primaryKey.Properties)
            {
                var propertyEntry = entityEntry.Properties.Where(a => a.Metadata == item).First();
                param.Add(item.Name, propertyEntry.CurrentValue);
            }
            return (sql, param);
        }
    }
}
