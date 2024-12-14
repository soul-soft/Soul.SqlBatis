﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Soul.SqlBatis.ChangeTracking
{
    public interface IEntityPersister
    {
        int SaveChanges(IDatabaseCommand command, IEnumerable<IEntityEntry> entries);
        Task<int> SaveChangesAsync(IDatabaseCommand command, IEnumerable<IEntityEntry> entries);
    }

    internal class EntityPersister : IEntityPersister
    {
        public int SaveChanges(IDatabaseCommand command, IEnumerable<IEntityEntry> entries)
        {
            var affectedRows = 0;
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    affectedRows += Insert(command, entry);
                }
                else if (entry.State == EntityState.Modified)
                {
                    affectedRows += Update(command, entry);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    affectedRows += Delete(command, entry);
                }
            }
            return affectedRows;
        }

        public async Task<int> SaveChangesAsync(IDatabaseCommand command, IEnumerable<IEntityEntry> entries)
        {
            var affectedRows = 0;
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    affectedRows += await InsertAsync(command, entry);
                }
                else if (entry.State == EntityState.Modified)
                {
                    affectedRows += await UpdateAsync(command, entry);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    affectedRows += await DeleteAsync(command, entry);
                }
            }
            return affectedRows;
        }

        private int Insert(IDatabaseCommand command, IEntityEntry entry)
        {
            var (sql, param) = BuildInsertCommand(entry);
            if (entry.Members.Any(a => a.IsIdentity()))
            {
                var obj = command.ExecuteScalar(sql, param);
                var identityProperty = entry.Members.Where(a => a.IsIdentity()).First();
                identityProperty.SetValue(obj);
                return 1;
            }
            else
            {
                return command.Execute(sql, param);
            }
        }

        private int Update(IDatabaseCommand command, IEntityEntry entry)
        {
            var (sql, param) = BuildUpdateCommand(entry);
            return command.Execute(sql, param);
        }

        private int Delete(IDatabaseCommand command, IEntityEntry entry)
        {
            var (sql, param) = BuildDeleteCommand(entry);
            return command.Execute(sql, param);
        }

        private async Task<int> InsertAsync(IDatabaseCommand command, IEntityEntry entry)
        {
            var (sql, param) = BuildInsertCommand(entry);
            if (entry.Members.Any(a => a.IsIdentity()))
            {
                var obj = await command.ExecuteScalarAsync(sql, param);
                var identityProperty = entry.Members.Where(a => a.IsIdentity()).First();
                identityProperty.SetValue(obj);
                return 1;
            }
            else
            {
                return await command.ExecuteAsync(sql, param);
            }
        }

        private Task<int> UpdateAsync(IDatabaseCommand command, IEntityEntry entry)
        {
            var (sql, param) = BuildUpdateCommand(entry);
            return command.ExecuteAsync(sql, param);
        }

        private Task<int> DeleteAsync(IDatabaseCommand command, IEntityEntry entry)
        {
            var (sql, param) = BuildDeleteCommand(entry);
            return command.ExecuteAsync(sql, param);
        }

        private (string, DynamicParameters) BuildInsertCommand(IEntityEntry entry)
        {
            var members = entry.Members.Where(a => !a.IsIdentity() && !a.IsNotMapped()).ToList();
            var columns = string.Join(",", members.Select(s => s.ColumnName));
            var paramNames = string.Join(",", members.Select(s => $"@{s.Property.Name}"));
            var sql = $"INSERT INTO {entry.TableName}({columns}) VALUES ({paramNames})";
            if (entry.Members.Any(a => a.IsIdentity()))
            {
                sql += ";SELECT LAST_INSERT_ID()";
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