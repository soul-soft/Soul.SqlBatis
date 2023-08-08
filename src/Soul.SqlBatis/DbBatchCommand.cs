using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Soul.SqlBatis.Infrastructure
{
	internal class DbBatchCommand
	{
		private readonly DbContext _context;

		public DbBatchCommand(DbContext context)
		{
			_context = context;
		}

		public int SaveChanges()
		{
			using (var transaction = _context.CurrentDbTransaction ?? _context.BeginTransaction())
			{
				var row = 0;
				foreach (var entry in _context.ChangeTracker.Entries())
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
				transaction.CommitTransaction();
				return row;
			}
		}

		public async Task<int> SaveChangesAsync()
		{
			var row = 0;
			using (var transaction = _context.CurrentDbTransaction ?? await _context.BeginTransactionAsync())
			{
				foreach (var entry in _context.ChangeTracker.Entries())
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
				await transaction.CommitTransactionAsync();
			}
			return row;
		}

		private int ExecuteInsert(EntityEntry entry)
		{
			var entityType = _context.Model.GetEntityType(entry.Entity.GetType());
			var sql = BuildInsertSql(entityType);
			var values = entry.Properties.ToDictionary(s => s.Member.Name, s => s.CurrentValue);
			if (!entityType.Properties.Any(a => a.IsIdentity))
			{
				return _context.Execute(sql, values);
			}
			var id = _context.ExecuteScalar<long>(sql, values);
			var identityProperty = entityType.Properties
				.Where(a => a.IsIdentity)
				.First().Member as PropertyInfo;
			SetIdentityPropertyValue(entry.Entity, identityProperty, id);
			return 1;
		}

		private async Task<int> ExecuteInsertAsync(EntityEntry entry)
		{
			var entityType = _context.Model.GetEntityType(entry.Entity.GetType());
			var sql = BuildInsertSql(entityType);
			var values = entry.Properties.ToDictionary(s => s.Member.Name, s => s.CurrentValue);
			if (!entityType.Properties.Any(a => a.IsIdentity))
			{
				return await _context.ExecuteAsync(sql, values);
			}
			var id = await _context.ExecuteScalarAsync<long>(sql, values);
			var identityProperty = entityType.Properties
				.Where(a => a.IsIdentity)
				.First().Member as PropertyInfo;
			SetIdentityPropertyValue(entry.Entity, identityProperty, id);
			return 1;
		}

		private int ExecuteUpdate(EntityEntry entry)
		{
			var entityType = _context.Model.GetEntityType(entry.Entity.GetType());
			var sql = BuildUpdateSql(entityType);
			var values = entry.Properties.ToDictionary(s => s.Member.Name, s => s.CurrentValue);
			return _context.Execute(sql, values);
		}

		private Task<int> ExecuteUpdateAsync(EntityEntry entry)
		{
			var entityType = _context.Model.GetEntityType(entry.Entity.GetType());
			var sql = BuildUpdateSql(entityType);
			var values = entry.Properties.ToDictionary(s => s.Member.Name, s => s.CurrentValue);
			return _context.ExecuteAsync(sql, values);
		}

		private int ExecuteDelete(EntityEntry entry)
		{
			var entityType = _context.Model.GetEntityType(entry.Entity.GetType());
			var sql = BuildDeleteSql(entityType);
			var values = entry.Properties.ToDictionary(s => s.Member.Name, s => s.CurrentValue);
			return _context.Execute(sql, values);
		}

		private Task<int> ExecuteDeleteAsync(EntityEntry entry)
		{
			var entityType = _context.Model.GetEntityType(entry.Entity.GetType());
			var sql = BuildDeleteSql(entityType);
			var values = entry.Properties.ToDictionary(s => s.Member.Name, s => s.CurrentValue);
			return _context.ExecuteAsync(sql, values);
		}

		private static void SetIdentityPropertyValue(object obj,PropertyInfo property,object value)
		{
			var type = Nullable.GetUnderlyingType(property.PropertyType)
				?? property.PropertyType;
			property.SetValue(obj, Convert.ChangeType(value, type));
		}

		private static string BuildInsertSql(EntityType entityType)
		{
			var properties = entityType.Properties.Where(a => !a.IsNotMapped);
			var columns = properties.Select(s => s.ColumnName);
			var parameters = properties.Select(s => $"@{s.Member.Name}");
			var sql = $"INSERT INTO {entityType.TableName} ({string.Join(",", columns)}) VALUES ({string.Join(",", parameters)})";
			if (properties.Any(a => a.IsIdentity))
			{
				return $"{sql};SELECT LAST_INSERT_ID();";
			}
			return sql;
		}

		private static string BuildUpdateSql(EntityType entityType)
		{
			var properties = entityType.Properties.Where(a => !a.IsNotMapped);
			var columns = properties.Where(a => !a.IsKey).Select(s => $"{s.ColumnName} = @{s.Member.Name}");
			var wheres = BuildWhereSql(entityType);
			var sql = $"UPDATE {entityType.TableName} SET {string.Join(", ", columns)} WHERE {string.Join(" AND ", wheres)}";
			return sql;
		}

		private static string BuildDeleteSql(EntityType entityType)
		{
			var properties = entityType.Properties.Where(a => !a.IsNotMapped).Where(a => a.IsKey);
			var wheres = BuildWhereSql(entityType);
			var parameters = properties.Select(s => $"@{s.Member.Name}");
			var sql = $"DELETE FROM {entityType.TableName} WHERE {wheres}";
			return sql;
		}

		private static string BuildWhereSql(EntityType entityType)
		{
			var keys = entityType.Properties
				.Where(a => !a.IsNotMapped)
				.Where(a => a.IsKey);
			if (!keys.Any())
			{
				throw new ModelException(string.Format("Primary key not found in '{0}' type", entityType.Type.Name));
			}
			return string.Join(" AND ", keys.Select(s => $"{s.ColumnName} = @{s.Member.Name}"));
		}
	}
}
