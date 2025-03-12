using System;
using System.Linq.Expressions;

namespace Soul.SqlBatis
{
    public interface IDbQueryable<T>
    {
        IDbQueryable<T> As(string name);
        IDbQueryable<T> Take(int count);
        IDbQueryable<T> Skip(int offset);
        IDbQueryable<T> AsTracking();
        IDbQueryable<T> AsNoTracking();
        IDbQueryable<T> Where(string expression, bool flag = true);
        IDbQueryable<T> Where(Expression<Func<T, bool>> expression, bool flag = true);
        IDbQueryable<T> OrderBy(string expression, bool flag = true);
        IDbQueryable<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression, bool flag = true);
        IDbQueryable<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression, bool flag = true);
        IDbQueryable<T> GroupBy(string expression, bool flag = true);
        IDbQueryable<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression, bool flag = true);
        IDbQueryable<T> Having(string expression, bool flag = true);
        IDbQueryable<T> Having<TResult>(Expression<Func<T, TResult>> expression, bool flag = true);
        IDbQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression);
        IDbQueryable<TResult> Clone<TResult>();
        (SqlBuilder, DynamicParameters) Build(Action<DbQueryableOptions> configureOptions = null);
    }
}
