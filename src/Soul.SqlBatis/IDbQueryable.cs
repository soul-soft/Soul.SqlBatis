using System;
using System.Linq.Expressions;

namespace Soul.SqlBatis
{
    public interface IDbQueryable<T>
    {
        IDbQueryable<T> FromSql(string sql, bool flag = true);
        IDbQueryable<TResult> Select<TResult>(string sql, bool flag = true);
        IDbQueryable<T> Where(string sql, object param = null, bool flag = true);
        IDbQueryable<T> Having(string sql, object param = null, bool flag = true);
        IDbQueryable<T> GroupBy(string sql, bool flag = true);
        IDbQueryable<T> OrderBy(string sql, bool flag = true);
        IDbQueryable<T> OrderByDescending(string sql, bool flag = true);
        IDbQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool flag = true);
        IDbQueryable<T> Where(Expression<Func<T, bool>> expression, bool flag = true);
        IDbQueryable<T> Having(Expression<Func<T, bool>> expression, bool flag = true);
        IDbQueryable<T> GroupBy(Expression<Func<T, bool>> expression, bool flag = true);
        IDbQueryable<T> OrderBy(Expression<Func<T, bool>> expression, bool flag = true);
        IDbQueryable<T> OrderByDescending(Expression<Func<T, bool>> expression, bool flag = true);
        IDbQueryable<T> Take(int count, bool flag = true);
        IDbQueryable<T> Skip(int count, bool flag = true);		
    }
}
