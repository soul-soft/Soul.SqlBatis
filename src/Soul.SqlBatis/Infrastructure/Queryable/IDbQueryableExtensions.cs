using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
	public static class IDbQueryableExtensions
	{

		private static DbQueryable AsQueryable<T>(this IDbQueryable<T> queryable)
		{
			if (!(queryable is DbQueryable))
			{
				throw new InvalidOperationException();
			}
			return queryable as DbQueryable;
		}

		public static List<T> ToList<T>(this IDbQueryable<T> queryable)
			where T : class
		{
			var query = queryable.AsQueryable();
			return query.ToList<T>();
		}

		public static Task<List<T>> ToListAsync<T>(this IDbQueryable<T> queryable)
		{
			var query = queryable.AsQueryable();
			return query.ToListAsync<T>();
		}

		public static int Count<T>(this IDbQueryable<T> queryable)
		{
			var query = queryable.AsQueryable();
			return query.Count();
		}

		public static async Task<int> CountAsync<T>(this IDbQueryable<T> queryable)
		{
			var query = queryable.AsQueryable();
			return await query.CountAsync();
		}

		public static int Count<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
		{
			queryable.Select(expression);
			var query = queryable.AsQueryable();
			return query.Count();
		}

		public static async Task<int> CountAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
		{
			queryable.Select(expression);
			var query = queryable.AsQueryable();
			return await query.CountAsync();
		}

		public static TProperty Sum<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
		{
			queryable.Select(expression);
			var query = queryable.AsQueryable();
			return query.Sum<TProperty>();
		}

		public static async Task<TProperty> SumAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
		{
			queryable.Select(expression);
			var query = queryable.AsQueryable();
			return await query.SumAsync<TProperty>();
		}

		public static TProperty Avg<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
		{
			queryable.Select(expression);
			var query = queryable.AsQueryable();
			return query.Avg<TProperty>();
		}

		public static async Task<TProperty> AvgAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
		{
			queryable.Select(expression);
			var query = queryable.AsQueryable();
			return await query.AvgAsync<TProperty>();
		}
	}
}
