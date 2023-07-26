using System;
using System.Linq.Expressions;

namespace Soul.SqlBatis
{
    public interface IDbQueryable<T>
    {
        IDbQueryable<T> FromSql(DbSyntax sql, bool flag = true);
        IDbQueryable<TResult> Select<TResult>(DbSyntax sql, bool flag = true);
        IDbQueryable<T> Where(DbSyntax sql, object param = null, bool flag = true);
        IDbQueryable<T> Having(DbSyntax sql, object param = null, bool flag = true);
        IDbQueryable<T> GroupBy(DbSyntax sql, bool flag = true);
        IDbQueryable<T> OrderBy(DbSyntax sql, bool flag = true);
        IDbQueryable<T> OrderByDescending(DbSyntax sql, bool flag = true);
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
