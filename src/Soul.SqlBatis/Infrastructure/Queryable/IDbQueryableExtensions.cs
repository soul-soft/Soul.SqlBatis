using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Soul.SqlBatis.Infrastructure;

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

        public static IDbQueryable<T> AsNoTracking<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            query.AsNoTracking();
            return queryable;
        }

        public static T First<T>(this IDbQueryable<T> queryable)
        {
            queryable.Take(1);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            var result = query.Query<T>(sql.QuerySql, sql.Parameters);
            return result.First();
        }

        public static T Single<T>(this IDbQueryable<T> queryable)
        {
            return queryable.First();
        }

        public static async Task<T> FirstAsync<T>(this IDbQueryable<T> queryable, CancellationToken? cancellationToken = null)
        {
            queryable.Take(1);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            var result = await query.QueryAsync<T>(sql.QuerySql, sql.Parameters, cancellationToken);
            return result.First();
        }

        public static async Task<T> SingleAsync<T>(this IDbQueryable<T> queryable, CancellationToken? cancellationToken = null)
        {
            return await queryable.FirstAsync(cancellationToken);
        }

        public static T FirstOrDefault<T>(this IDbQueryable<T> queryable)
        {
            queryable.Take(1);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            var result = query.Query<T>(sql.QuerySql, sql.Parameters);
            return result.FirstOrDefault();
        }

        public static T SingleOrDefault<T>(this IDbQueryable<T> queryable)
        {
            return queryable.FirstOrDefault();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IDbQueryable<T> queryable, CancellationToken? cancellationToken = null)
        {
            queryable.Take(1);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            var result = await query.QueryAsync<T>(sql.QuerySql, sql.Parameters, cancellationToken);
            return result.FirstOrDefault();
        }

        public static async Task<T> SingleOrDefaultAsync<T>(this IDbQueryable<T> queryable, CancellationToken? cancellationToken = null)
        {
            return await queryable.FirstOrDefaultAsync(cancellationToken);
        }

        public static List<T> ToList<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            var result = query.Query<T>(sql.QuerySql, sql.Parameters);
            return result;
        }

        public static async Task<List<T>> ToListAsync<T>(this IDbQueryable<T> queryable, CancellationToken? cancellationToken = null)
        {
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            var result = await query.QueryAsync<T>(sql.QuerySql, sql.Parameters, cancellationToken);
            return result;
        }

        public static (List<T>, int) ToPageList<T>(this IDbQueryable<T> queryable, int page, int size)
        {
            var offset = (page - 1) * size;
            var list = queryable.Take(size).Skip(offset).ToList();
            var count = queryable.Count();
            return (list, count);
        }

        public static async Task<(List<T>, int)> ToPageListAsync<T>(this IDbQueryable<T> queryable, int page, int size, CancellationToken? cancellationToken = null)
        {
            var offset = (page - 1) * size;
            var list = await queryable.Take(size).Skip(offset).ToListAsync(cancellationToken);
            var count = await queryable.CountAsync(cancellationToken);
            return (list, count);
        }

        public static int Count<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.ExecuteScalar<int>(sql.CountQuerySql, sql.Parameters);
        }

        public static async Task<int> CountAsync<T>(this IDbQueryable<T> queryable, CancellationToken? cancellationToken = null)
        {
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return await query.ExecuteScalarAsync<int>(sql.CountQuerySql, sql.Parameters, cancellationToken);
        }

        public static long LongCount<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.ExecuteScalar<long>(sql.CountQuerySql, sql.Parameters);
        }

        public static async Task<long> LongCountAsync<T>(this IDbQueryable<T> queryable, CancellationToken? cancellationToken = null)
        {
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return await query.ExecuteScalarAsync<long>(sql.CountQuerySql, sql.Parameters, cancellationToken);
        }

        public static int Count<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.ExecuteScalar<int>(sql.CountQuerySql, sql.Parameters);
        }

        public static async Task<int> CountAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression, CancellationToken? cancellationToken = null)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return await query.ExecuteScalarAsync<int>(sql.CountQuerySql, sql.Parameters, cancellationToken);
        }

        public static TProperty Sum<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.ExecuteScalar<TProperty>(sql.SumQuerySql, sql.Parameters);
        }

        public static async Task<TProperty> SumAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression, CancellationToken? cancellationToken = null)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return await query.ExecuteScalarAsync<TProperty>(sql.SumQuerySql, sql.Parameters, cancellationToken);
        }

        public static TProperty Average<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.ExecuteScalar<TProperty>(sql.AvgQuerySql, sql.Parameters);
        }

        public static async Task<TProperty> AverageAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression, CancellationToken? cancellationToken = null)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return await query.ExecuteScalarAsync<TProperty>(sql.AvgQuerySql, sql.Parameters, cancellationToken);
        }

        public static TProperty Max<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.ExecuteScalar<TProperty>(sql.MaxQuerySql, sql.Parameters);
        }

        public static async Task<TProperty> MaxAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression, CancellationToken? cancellationToken = null)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return await query.ExecuteScalarAsync<TProperty>(sql.MaxQuerySql, sql.Parameters, cancellationToken);
        }

        public static TProperty Min<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.ExecuteScalar<TProperty>(sql.MinQuerySql, sql.Parameters);
        }

        public static async Task<TProperty> MinAsync<T, TProperty>(this IDbQueryable<T> queryable, Expression<Func<T, TProperty>> expression, CancellationToken? cancellationToken = null)
        {
            queryable.Select(expression);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return await query.ExecuteScalarAsync<TProperty>(sql.MinQuerySql, sql.Parameters, cancellationToken);
        }

        public static bool Any<T>(this IDbQueryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            queryable.Where(expression);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.ExecuteScalar<bool>(sql.AnyQuerySql, sql.Parameters);
        }

        public static bool Any<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.ExecuteScalar<bool>(sql.AnyQuerySql, sql.Parameters);
        }

        public static Task<bool> AnyAsync<T>(this IDbQueryable<T> queryable, Expression<Func<T, bool>> expression, CancellationToken? cancellationToken = null)
        {
            queryable.Where(expression);
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.ExecuteScalarAsync<bool>(sql.AnyQuerySql, sql.Parameters, cancellationToken);
        }

        public static Task<bool> AnyAsync<T>(this IDbQueryable<T> queryable, CancellationToken? cancellationToken = null)
        {
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.ExecuteScalarAsync<bool>(sql.AnyQuerySql, sql.Parameters, cancellationToken);
        }

        public static int ExecuteUpdate<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.Execute(sql.UpdateSql, sql.Parameters);
        }

        public static Task<int> ExecuteUpdateAsync<T>(this IDbQueryable<T> queryable, CancellationToken? cancellationToken = null)
        {
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.ExecuteAsync(sql.UpdateSql, sql.Parameters, cancellationToken: cancellationToken);
        }

        public static int ExecuteDelete<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.Execute(sql.DeleteSql, sql.Parameters);
        }

        public static Task<int> ExecuteDeleteAsync<T>(this IDbQueryable<T> queryable, CancellationToken? cancellationToken = null)
        {
            var query = queryable.AsQuery();
            var sql = query.BuildSqlQuery();
            return query.ExecuteAsync(sql.DeleteSql, sql.Parameters, cancellationToken: cancellationToken);
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
