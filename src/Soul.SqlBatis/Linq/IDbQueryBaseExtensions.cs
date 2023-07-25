using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Soul.SqlBatis
{
	public static class DbQueryBaseExtensions
	{
		public static List<T> ToList<T>(this IDbQueryable<T> queryable)
		{
			if (!(queryable is DbQueryBase))
			{
				throw new InvalidOperationException();
			}
			var query = queryable as DbQueryBase;
			return ExecuteQuery<T>(query).ToList();
		}

		public static async Task<List<T>> ToListAsync<T>(this IDbQueryable<T> queryable)
		{
			if (!(queryable is DbQueryBase))
			{
				throw new InvalidOperationException();
			}
			var query = queryable as DbQueryBase;
			return (await ExecuteQueryAsync<T>(query)).ToList();
		}

		public static T[] ToArray<T>(this IDbQueryable<T> queryable)
		{
			if (!(queryable is DbQueryBase))
			{
				throw new InvalidOperationException();
			}
			var query = queryable as DbQueryBase;
			return ExecuteQuery<T>(query).ToArray();
		}

		public static async Task<T[]> ToArrayAsync<T>(this IDbQueryable<T> queryable)
		{
			if (!(queryable is DbQueryBase))
			{
				throw new InvalidOperationException();
			}
			var query = queryable as DbQueryBase;
			return (await ExecuteQueryAsync<T>(query)).ToArray();
		}

		private static IEnumerable<T> ExecuteQuery<T>(DbQueryBase query)
		{
			var command = query.Build();
			var context = query.DbContext;
			var connection = context.GetDbConnection();
			var transaction = context.GetDbTransaction();
			return connection.Query<T>(command.CommandText, command.Parameters, transaction);
		}

		private static Task<IEnumerable<T>> ExecuteQueryAsync<T>(DbQueryBase query)
		{
			var command = query.Build();
			var context = query.DbContext;
			var connection = context.GetDbConnection();
			var transaction = context.GetDbTransaction();
			return connection.QueryAsync<T>(command.CommandText, command.Parameters, transaction);
		}
	}
}
