using Soul.SqlBatis.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soul.SqlBatis.Infrastructure
{
    internal class DbContextUnitWork
    {
        private readonly DbContext _context;
        private readonly SqlSettings _sqlSettings;
        
        public DbContextUnitWork(DbContext context,SqlSettings sqlSettings)
        {
            _context = context;
            _sqlSettings = sqlSettings;
        }

        public int SaveChanges(IEnumerable<IEntityEntry> entries)
        {
            var affectedRows = 0;
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    affectedRows += Insert(entry);
                }
                else if (entry.State == EntityState.Modified)
                {
                    affectedRows += Update(entry);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    affectedRows += Delete(entry);
                }
            }
            return affectedRows;
        }

        public async Task<int> SaveChangesAsync(IEnumerable<IEntityEntry> entries)
        {
            var affectedRows = 0;
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    affectedRows += await InsertAsync(entry);
                }
                else if (entry.State == EntityState.Modified)
                {
                    affectedRows += await UpdateAsync(entry);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    affectedRows += await DeleteAsync( entry);
                }
            }
            return affectedRows;
        }

        private int Insert(IEntityEntry entry)
        {
            var (sql, param) = BuildInsertCommand(entry);
            if (entry.Members.Any(a => a.IsIdentity()))
            {
                var obj = _context.Sql.ExecuteScalar(sql, param);
                var identityProperty = entry.Members.Where(a => a.IsIdentity()).First();
                identityProperty.SetValue(obj);
                return 1;
            }
            else
            {
                return _context.Sql.Execute(sql, param);
            }
        }

        private int Update(IEntityEntry entry)
        {
            var (sql, param) = BuildUpdateCommand(entry);
            return _context.Sql.Execute(sql, param);
        }

        private int Delete(IEntityEntry entry)
        {
            var (sql, param) = BuildDeleteCommand(entry);
            return _context.Sql.Execute(sql, param);
        }

        private async Task<int> InsertAsync(IEntityEntry entry)
        {
            var (sql, param) = BuildInsertCommand(entry);
            if (entry.Members.Any(a => a.IsIdentity()))
            {
                var obj = await _context.Sql.ExecuteScalarAsync(sql, param);
                var identityProperty = entry.Members.Where(a => a.IsIdentity()).First();
                identityProperty.SetValue(obj);
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

        private (string, DynamicParameters) BuildInsertCommand(IEntityEntry entry)
        {
            var members = entry.Members.Where(a => !a.IsIdentity() && !a.IsNotMapped()).ToList();
            var columns = string.Join(",", members.Select(s => s.ColumnName));
            var paramNames = string.Join(",", members.Select(s => $"@{s.Property.Name}"));
            var sql = $"INSERT INTO {entry.TableName} ({columns}) VALUES ({paramNames})";
            if (entry.Members.Any(a => a.IsIdentity()))
            {
                var column = entry.Members.Where(a => a.IsIdentity()).First();
                sql += $"{string.Format(_sqlSettings.IdentitySql, column.ColumnName)}";
            }
            var param = new DynamicParameters();
            foreach (var item in members)
            {
                param.Add(item.Property.Name, item.CurrentValue);
            }
            return (sql, param);
        }

        private (string, DynamicParameters) BuildUpdateCommand(IEntityEntry entry)
        {
            var members = entry.Members.Where(a => !a.IsIdentity() && !a.IsNotMapped() && !a.IsKey() && a.IsChanged).ToList();
            var columns = string.Join(",", members.Select(s => $"{s.ColumnName} = @{s.Property.Name}"));
            var keyMembers = entry.Members.Where(a => a.IsKey());
            var whereSql = string.Join(" AND ", keyMembers.Select(s => $"{s.ColumnName} = @{s.Property.Name}"));
            var sql = $"UPDATE {entry.TableName} SET {columns} WHERE {whereSql}";
            var param = new DynamicParameters();
            foreach (var item in members.Union(keyMembers))
            {
                param.Add(item.Property.Name, item.CurrentValue);
            }
            return (sql, param);
        }

        private (string, DynamicParameters) BuildDeleteCommand(IEntityEntry entry)
        {
            var keyMembers = entry.Members.Where(a => a.IsKey());
            var whereSql = string.Join(" AND ", keyMembers.Select(s => $"{s.ColumnName} = @{s.Property.Name}"));
            var sql = $"DELETE FROM {entry.TableName} WHERE {whereSql}";
            var param = new DynamicParameters();
            foreach (var item in keyMembers)
            {
                param.Add(item.Property.Name, item.CurrentValue);
            }
            return (sql, param);
        }
    }
}
