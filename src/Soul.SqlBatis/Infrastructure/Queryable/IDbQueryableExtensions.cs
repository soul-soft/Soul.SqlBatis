using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
    public static class IDbQueryableExtensions
    {
        public static IDbQueryable<T> AsTracking<T>(this IDbQueryable<T> queryable)
            where T : class
        {
            var query = queryable.AsQuery();
            query.AsTracking();
            return queryable;
        }

        public static T First<T>(this IDbQueryable<T> queryable)
           where T : class
        {
            queryable.Take(1);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildSelect();
            var result = query.Query<T>(sql, param);
            return result.First();
        }

        public static async Task<T> FirstAsync<T>(this IDbQueryable<T> queryable)
        {
            queryable.Take(1);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildSelect();
            var result = await query.QueryAsync<T>(sql, param);
            return result.First();
        }

        public static T FirstOrDefault<T>(this IDbQueryable<T> queryable)
        {
            queryable.Take(1);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildSelect();
            var result = query.Query<T>(sql, param);
            return result.FirstOrDefault();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IDbQueryable<T> queryable)
        {
            queryable.Take(1);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildSelect();
            var result = await query.QueryAsync<T>(sql, param);
            return result.FirstOrDefault();
        }

        public static List<T> ToList<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildSelect();
            var result = query.Query<T>(sql, param);
            return result;
        }

        public static async Task<List<T>> ToListAsync<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildSelect();
            var result = await query.QueryAsync<T>(sql, param);
            return result;
        }

        public static (List<T>, int) ToPageList<T>(this IDbQueryable<T> queryable,int page,int size)
        {
            var offset = (page-1) * size;
            var list = queryable.Take(size).Skip(offset).ToList();
            var count = queryable.Count();
            return (list, count);
        }

        public static async Task<(List<T>, int)> ToPageListAsync<T>(this IDbQueryable<T> queryable, int page, int size)
        {
            var offset = (page - 1) * size;
            var list = await queryable.Take(size).Skip(offset).ToListAsync();
            var count = await queryable.CountAsync();
            return (list, count);
        }

        public static int Count<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildCount();
            return query.ExecuteScalar<int>(sql, param);
        }

        public static async Task<int> CountAsync<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildCount();
            return await query.ExecuteScalarAsync<int>(sql, param);
        }

        public static long LongCount<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildCount();
            return query.ExecuteScalar<long>(sql, param);
        }

        public static async Task<long> LongCountAsync<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildCount();
            return await query.ExecuteScalarAsync<long>(sql, param);
        }

        public static int Count<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildCount();
            return query.ExecuteScalar<int>(sql, param);
        }

        public static async Task<int> CountAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildCount();
            return await query.ExecuteScalarAsync<int>(sql, param);
        }

        public static TProperty Sum<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildSum();
            return query.ExecuteScalar<TProperty>(sql, param);
        }

        public static async Task<TProperty> SumAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildSum();
            return await query.ExecuteScalarAsync<TProperty>(sql, param);
        }

        public static TProperty Average<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildAverage();
            return query.ExecuteScalar<TProperty>(sql, param);
        }

        public static async Task<TProperty> Averagesync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildAverage();
            return await query.ExecuteScalarAsync<TProperty>(sql, param);
        }

        public static TProperty Max<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildMax();
            return query.ExecuteScalar<TProperty>(sql, param);
        }

        public static async Task<TProperty> MaxAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildMax();
            return await query.ExecuteScalarAsync<TProperty>(sql, param);
        }

        public static TProperty Min<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildMin();
            return query.ExecuteScalar<TProperty>(sql, param);
        }

        public static async Task<TProperty> MinAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildMin();
            return await query.ExecuteScalarAsync<TProperty>(sql, param);
        }

        public static bool Any<T>(this IDbQueryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            queryable.Where(expression);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildAny();
            return query.ExecuteScalar<bool>(sql, param);
        }

        public static Task<bool> AnyAsync<T>(this IDbQueryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            queryable.Where(expression);
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildAny();
            return query.ExecuteScalarAsync<bool>(sql, param);
        }

        public static int ExecuteUpdate<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildUpdate();
            return query.Execute(sql, param);
        }

        public static Task<int> ExecuteUpdateAsync<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildUpdate();
            return query.ExecuteAsync(sql, param);
        }

        public static int ExecuteDelete<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildDelete();
            return query.Execute(sql, param);
        }

        public static Task<int> ExecuteDeleteAsync<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var (sql, param) = query.BuildDelete();
            return query.ExecuteAsync(sql, param);
        }

        private static DbQueryable AsQuery<T>(this IDbQueryable<T> queryable)
        {
            if (!(queryable is DbQueryable))
            {
                throw new InvalidOperationException();
            }
            return queryable as DbQueryable;
        }
    }
}
