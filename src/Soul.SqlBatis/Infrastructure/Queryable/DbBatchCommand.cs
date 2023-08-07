using System.Linq;

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
			var row = 0;
			foreach (var entry in _context.ChangeTracker.Entries())
			{
				if (entry.State == EntityState.Added)
				{
					var sql = InsertSql(_context, entry);
					var values = entry.Properties.ToDictionary(s => s.Member.Name, s => s.CurrentValue);
					row += _context.Execute(sql, values);
				}
				else if (entry.State == EntityState.Modified)
				{
					var sql = UpdateSql(_context, entry);
					var values = entry.Properties.ToDictionary(s => s.Member.Name, s => s.CurrentValue);
					row += _context.Execute(sql, values);
				}
				else if (entry.State == EntityState.Deleted)
				{
					var sql = DeleteSql(_context, entry);
					var values = entry.Properties.ToDictionary(s => s.Member.Name, s => s.CurrentValue);
					row += _context.Execute(sql, values);
				}
			}
			return row;
		}

		private static string InsertSql(DbContext context, EntityEntry entry)
		{
			var entityType = context.Model.GetEntityType(entry.Entity.GetType());
			var properties = entityType.Properties.Where(a => !a.IsNotMapped);
			var columns = properties.Select(s => s.ColumnName);
			var parameters = properties.Select(s => $"@{s.Member.Name}");
			var sql = $"INSERT INTO {entityType.TableName} ({string.Join(",", columns)}) VALUES ({string.Join(",", parameters)})";
			return sql;
		}

		private static string UpdateSql(DbContext context, EntityEntry entry)
		{
			var entityType = context.Model.GetEntityType(entry.Entity.GetType());
			var properties = entityType.Properties.Where(a => !a.IsNotMapped);
			var columns = properties.Where(a => !a.IsKey).Select(s => $"{s.ColumnName} = @{s.Member.Name}");
			var wheres = WhereSql(entityType);
			var sql = $"UPDATE {entityType.TableName} SET {string.Join(", ", columns)} WHERE {string.Join(" AND ", wheres)}";
			return sql;
		}

		private static string DeleteSql(DbContext context, EntityEntry entry)
		{
			var entityType = context.Model.GetEntityType(entry.Entity.GetType());
			var properties = entityType.Properties.Where(a => !a.IsNotMapped).Where(a => a.IsKey);
			var wheres = WhereSql(entityType);
			var parameters = properties.Select(s => $"@{s.Member.Name}");
			var sql = $"DELETE FROM {entityType.TableName} WHERE {wheres}";
			return sql;
		}

		private static string WhereSql(EntityType entityType)
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
