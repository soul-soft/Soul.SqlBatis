using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
    public static class IDbQueryableExtensions
    {
        public static T First<T>(this IDbQueryable<T> queryable)
           where T : class
        {
            queryable.Take(1);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildSelect();
            var result = context.Query<T>(sql, param);
            return result.First();
        }

        public static async Task<T> FirstAsync<T>(this IDbQueryable<T> queryable)
        {
            queryable.Take(1);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildSelect();
            var result = await context.QueryAsync<T>(sql, param);
            return result.First();
        }

        public static T FirstOrDefault<T>(this IDbQueryable<T> queryable)
        {
            queryable.Take(1);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildSelect();
            var result = context.Query<T>(sql, param);
            return result.FirstOrDefault();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IDbQueryable<T> queryable)
        {
            queryable.Take(1);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildSelect();
            var result = await context.QueryAsync<T>(sql, param);
            return result.FirstOrDefault();
        }
      
        public static List<T> ToList<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildSelect();
            var result = context.Query<T>(sql, param);
            return result.ToList();
        }

        public static async Task<List<T>> ToListAsync<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildSelect();
            var result = await context.QueryAsync<T>(sql, param);
            return result.ToList();
        }

        public static int Count<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildCount();
            return context.ExecuteScalar<int>(sql, param);
        }

        public static async Task<int> CountAsync<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildCount();
            return await context.ExecuteScalarAsync<int>(sql, param);
        }

        public static int Count<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildCount();
            return context.ExecuteScalar<int>(sql, param);
        }

        public static async Task<int> CountAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildCount();
            return await context.ExecuteScalarAsync<int>(sql, param);
        }

        public static TProperty Sum<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildSum();
            return context.ExecuteScalar<TProperty>(sql, param);
        }

        public static async Task<TProperty> SumAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildSum();
            return await context.ExecuteScalarAsync<TProperty>(sql, param);
        }

        public static TProperty Average<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildAverage();
            return context.ExecuteScalar<TProperty>(sql, param);
        }

        public static async Task<TProperty> Averagesync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildAverage();
            return await context.ExecuteScalarAsync<TProperty>(sql, param);
        }

        public static TProperty Max<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildMax();
            return context.ExecuteScalar<TProperty>(sql, param);
        }

        public static async Task<TProperty> MaxAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildMax();
            return await context.ExecuteScalarAsync<TProperty>(sql, param);
        }

        public static TProperty Min<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildMin();
            return context.ExecuteScalar<TProperty>(sql, param);
        }

        public static async Task<TProperty> MinAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildMin();
            return await context.ExecuteScalarAsync<TProperty>(sql, param);
        }

        public static bool Any<T>(this IDbQueryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            queryable.Where(expression);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildAny();
            return context.ExecuteScalar<bool>(sql, param);
        }

        public static Task<bool> AnyAsync<T>(this IDbQueryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            queryable.Where(expression);
            var query = queryable.AsQueryable();
            var context = query.DbContext;
            var (sql, param) = query.BuildAny();
            return context.ExecuteScalarAsync<bool>(sql, param);
        }

        private static DbQueryable AsQueryable<T>(this IDbQueryable<T> queryable)
        {
            if (!(queryable is DbQueryable))
            {
                throw new InvalidOperationException();
            }
            return queryable as DbQueryable;
        }
    }
}
