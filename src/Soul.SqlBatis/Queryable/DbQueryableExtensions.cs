using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
    public static class DbQueryableExtensions
    {
        private static void Track<T>(this IDbQueryable<T> queryable, ref T entity)
        {
            var query = queryable.GetDbQueryable();
            if (!query.IsTracking || entity == null)
            {
                return;
            }
            var context = queryable.GetDbContext();
            if (!query.Tokens.Any(a => a.Key == DbQueryTokenType.Select))
            {
                entity = (T)context.Attach(entity).Entity;
            }
        }

        private static void Track<T>(this IDbQueryable<T> queryable, List<T> entities)
        {
            var query = queryable.GetDbQueryable();
            if (!query.IsTracking)
            {
                return;
            }
            if (entities == null || entities.Count == 0)
            {
                return;
            }
            var context = queryable.GetDbContext();
            if (!query.Tokens.Any(a => a.Key == DbQueryTokenType.Select))
            {
                for (var i = 0; i < entities.Count; i++)
                {
                    entities[i] = (T)context.Attach(entities[i]).Entity;
                }
            }
        }

        private static DbQueryable<T> GetDbQueryable<T>(this IDbQueryable<T> queryable)
        {
            if (queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }
            if (!(queryable is DbQueryable<T> dbQueryable))
            {
                throw new NotSupportedException();
            }
            return dbQueryable;
        }

        private static SqlMapper GetSqlMapper<T>(this IDbQueryable<T> queryable)
        {
            return queryable.GetDbContext().Sql;
        }

        private static DbContext GetDbContext<T>(this IDbQueryable<T> queryable)
        {
            var query = queryable.GetDbQueryable();
            return query.GetDbContext();
        }

        public static bool Any<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalar<bool>(sqler.AnySql, param);
        }

        public static Task<bool> AnyAsync<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalarAsync<bool>(sqler.AnySql, param);
        }

        public static bool Any<T>(this IDbQueryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            queryable.Where(expression);
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalar<bool>(sqler.AnySql, param);
        }

        public static Task<bool> AnyAsync<T>(this IDbQueryable<T> queryable, Expression<Func<T, bool>> expression)
        {
            queryable.Where(expression);
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalarAsync<bool>(sqler.AnySql, param);
        }

        public static T Min<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalar<T>(sqler.MinSql, param);
        }

        public static Task<T> MinAsync<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalarAsync<T>(sqler.MinSql, param);
        }

        public static TResult Min<T, TResult>(this IDbQueryable<T> queryable, Expression<Func<T, TResult>> expression)
        {
            queryable.Select(expression);
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalar<TResult>(sqler.MinSql, param);
        }

        public static Task<TResult> MinAsync<T, TResult>(this IDbQueryable<T> queryable, Expression<Func<T, TResult>> expression)
        {
            queryable.Select(expression);
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalarAsync<TResult>(sqler.MinSql, param);
        }


        public static T Max<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalar<T>(sqler.MaxSql, param);
        }

        public static Task<T> MaxAsync<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalarAsync<T>(sqler.MaxSql, param);
        }

        public static TResult Max<T, TResult>(this IDbQueryable<T> queryable, Expression<Func<T, TResult>> expression)
        {
            queryable.Select(expression);
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalar<TResult>(sqler.MaxSql, param);
        }

        public static Task<TResult> MaxAsync<T, TResult>(this IDbQueryable<T> queryable, Expression<Func<T, TResult>> expression)
        {
            queryable.Select(expression);
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalarAsync<TResult>(sqler.MaxSql, param);
        }

        public static T Average<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalar<T>(sqler.AvgSql, param);
        }

        public static Task<T> AverageAsync<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalarAsync<T>(sqler.AvgSql, param);
        }

        public static TResult Average<T, TResult>(this IDbQueryable<T> queryable, Expression<Func<T, TResult>> expression)
        {
            queryable.Select(expression);
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalar<TResult>(sqler.AvgSql, param);
        }

        public static Task<TResult> AverageAsync<T, TResult>(this IDbQueryable<T> queryable, Expression<Func<T, TResult>> expression)
        {
            queryable.Select(expression);
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalarAsync<TResult>(sqler.AvgSql, param);
        }

        public static T Sum<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalar<T>(sqler.SumSql, param);
        }

        public static Task<T> SumAsync<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalarAsync<T>(sqler.SumSql, param);
        }

        public static TResult Sum<T, TResult>(this IDbQueryable<T> queryable, Expression<Func<T, TResult>> expression)
        {
            queryable.Select(expression);
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalar<TResult>(sqler.SumSql, param);
        }

        public static Task<TResult> SumAsync<T, TResult>(this IDbQueryable<T> queryable, Expression<Func<T, TResult>> expression)
        {
            queryable.Select(expression);
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalarAsync<TResult>(sqler.SumSql, param);
        }

        public static int Count<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalar<int>(sqler.CountSql, param);
        }

        public static Task<int> CountAsync<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalarAsync<int>(sqler.CountSql, param);
        }

        public static int Count<T, TResult>(this IDbQueryable<T> queryable, Expression<Func<T, TResult>> expression)
        {
            queryable.Select(expression);
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalar<int>(sqler.CountSql, param);
        }

        public static Task<int> CountAsync<T, TResult>(this IDbQueryable<T> queryable, Expression<Func<T, TResult>> expression)
        {
            queryable.Select(expression);
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            return command.ExecuteScalarAsync<int>(sqler.CountSql, param);
        }

        public static T First<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build();
            var entity = command.QueryFirst<T>(sqler.QuerySql, param);
            if (entity == null)
            {
                throw new InvalidOperationException("The source sequence is empty.");
            }
            queryable.Track(ref entity);
            return entity;
        }

        public static T Single<T>(this IDbQueryable<T> queryable)
        {
            return First(queryable);
        }

        public static async Task<T> FirstAsync<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build();
            var entity = await command.QueryFirstAsync<T>(sqler.QuerySql, param);
            if (entity == null)
            {
                throw new InvalidOperationException("The source sequence is empty.");
            }
            queryable.Track(ref entity);
            return entity;
        }

        public static async Task<T> SingleAsync<T>(this IDbQueryable<T> queryable)
        {
            return await FirstAsync(queryable);
        }

        public static T FirstOrDefault<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build();
            var entity = command.QueryFirstOrDefault<T>(sqler.QuerySql, param);
            queryable.Track(ref entity);
            return entity;
        }

        public static T SingleOrDefault<T>(this IDbQueryable<T> queryable)
        {
            return FirstOrDefault(queryable);
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build();
            var entity = await command.QueryFirstOrDefaultAsync<T>(sqler.QuerySql, param);
            queryable.Track(ref entity);
            return entity;
        }

        public static async Task<T> SingleOrDefaultAsync<T>(this IDbQueryable<T> queryable)
        {
            return await FirstOrDefaultAsync(queryable);
        }

        public static List<T> ToList<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions => 
            {
                configureOptions.UseDefaultOrder = true;
            });
            var entities = command.Query<T>(sqler.QuerySql, param);
            queryable.Track(entities);
            return entities;
        }

        public static async Task<List<T>> ToListAsync<T>(this IDbQueryable<T> queryable)
        {
            var command = queryable.GetSqlMapper();
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.UseDefaultOrder = true;
            });
            var entities = await command.QueryAsync<T>(sqler.QuerySql, param);
            queryable.Track(entities);
            return entities;
        }
      
        public static (List<T>, int) ToPageResult<T>(this IDbQueryable<T> queryable, int pageIndex, int pageSize)
        {
            queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var (queryer, param) = queryable.Build(configureOptions =>
            {
                configureOptions.UseDefaultOrder = true;
            });
            var (counter, _) = queryable.Clone<int>().Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            var context = queryable.GetDbContext();
            var pageSql = $"{queryer.QuerySql};\r\n{counter.CountSql}";
            using (var grid = context.Sql.QueryMultiple(pageSql, param))
            {
                var list = grid.Read<T>();
                var total = grid.ReadFirst<int>();
                return (list, total);
            }
        }

        public static async Task<(List<T>, int)> ToPageResultAsync<T>(this IDbQueryable<T> queryable, int pageIndex, int pageSize)
        {
            queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var (queryer, param) = queryable.Build(configureOptions =>
            {
                configureOptions.UseDefaultOrder = true;
            });
            var (counter, _) = queryable.Clone<int>().Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            var context = queryable.GetDbContext();
            var pageSql = $"{queryer.QuerySql};\r\n{counter.CountSql}";
            using (var grid = await context.Sql.QueryMultipleAsync(pageSql, param))
            {
                var list = await grid.ReadAsync<T>();
                var total = await grid.ReadFirstAsync<int>();
                return (list, total);
            }
        }

        public static int ExecuteDelete<T>(this IDbQueryable<T> queryable)
        {
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            var command = queryable.GetSqlMapper();
            return command.Execute(sqler.DeleteSql, param);
        }

        public static Task<int> ExecuteDeleteAsync<T>(this IDbQueryable<T> queryable)
        {
            var (sqler, param) = queryable.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            var command = queryable.GetSqlMapper();
            return command.ExecuteAsync(sqler.DeleteSql, param);
        }

        public static int ExecuteUpdate<T>(this IDbQueryable<T> queryable, Expression<Func<DbUpdateQueryable<T>, DbUpdateQueryable<T>>> setters)
        {
            var query = queryable.GetDbQueryable();
            query.AddToken(DbQueryTokenType.Setters, setters);
            var (sqler, param) = query.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            var command = queryable.GetSqlMapper();
            return command.Execute(sqler.UpdateSql, param);
        }

        public static Task<int> ExecuteUpdateAsync<T>(this IDbQueryable<T> queryable, Expression<Func<DbUpdateQueryable<T>, DbUpdateQueryable<T>>> setters)
        {
            var query = queryable.GetDbQueryable();
            query.AddToken(DbQueryTokenType.Setters, setters);
            var (sqler, param) = query.Build(configureOptions =>
            {
                configureOptions.HasColumnsAlias = false;
                configureOptions.HasDefaultColumns = false;
            });
            var command = queryable.GetSqlMapper();
            return command.ExecuteAsync(sqler.UpdateSql, param);
        }
    }
}
