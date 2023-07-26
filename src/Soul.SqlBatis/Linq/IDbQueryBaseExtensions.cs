using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Soul.SqlBatis
{
    public static class IDbQueryBaseExtensions
	{
		public static List<T> ToList<T>(this IDbQueryable<T> queryable)
		{
			if (!(queryable is DbQuery))
			{
				throw new InvalidOperationException();
			}
			var query = queryable as DbQuery;
			return Query<T>(query).ToList();
		}

		public static async Task<List<T>> ToListAsync<T>(this IDbQueryable<T> queryable)
		{
			if (!(queryable is DbQuery))
			{
				throw new InvalidOperationException();
			}
			var query = queryable as DbQuery;
			return (await QueryAsync<T>(query)).ToList();
		}

		public static T[] ToArray<T>(this IDbQueryable<T> queryable)
		{
			if (!(queryable is DbQuery))
			{
				throw new InvalidOperationException();
			}
			var query = queryable as DbQuery;
			return Query<T>(query).ToArray();
		}

		public static async Task<T[]> ToArrayAsync<T>(this IDbQueryable<T> queryable)
		{
			if (!(queryable is DbQuery))
			{
				throw new InvalidOperationException();
			}
			var query = queryable as DbQuery;
			return (await QueryAsync<T>(query)).ToArray();
		}

		private static IEnumerable<T> Query<T>(DbQuery query)
		{
			var command = query.Build();
			var context = query.DbContext;
			var connection = context.GetDbConnection();
			var transaction = context.CurrentDbTransaction?.GetDbTransaction();
			return connection.Query<T>(command.CommandText, command.Parameters, transaction);
		}

		private static Task<IEnumerable<T>> QueryAsync<T>(DbQuery query)
		{
			var command = query.Build();
			var context = query.DbContext;
			var connection = context.GetDbConnection();
			var transaction = context.CurrentDbTransaction?.GetDbTransaction();
			return connection.QueryAsync<T>(command.CommandText, command.Parameters, transaction);
		}
	}
}
