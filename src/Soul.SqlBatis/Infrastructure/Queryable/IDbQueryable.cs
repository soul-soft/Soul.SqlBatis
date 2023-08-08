using System;
using System.Linq.Expressions;

namespace Soul.SqlBatis
{
    public interface IDbQueryable<T>
	{
		IDbQueryable<T> FromSql(DbSql sql, bool flag = true);
		IDbQueryable<TResult> Select<TResult>(DbSql sql, bool flag = true);
		IDbQueryable<T> Where(DbSql sql, object param = null, bool flag = true);
		IDbQueryable<T> Having(DbSql sql, object param = null, bool flag = true);
		IDbQueryable<T> GroupBy(DbSql sql, bool flag = true);
		IDbQueryable<T> OrderBy(DbSql sql, bool flag = true);
		IDbQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool flag = true);
		IDbQueryable<T> Where(Expression<Func<T, bool>> expression, bool flag = true);
		IDbQueryable<T> Having(Expression<Func<T, bool>> expression, bool flag = true);
		IDbQueryable<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression, bool flag = true);
		IDbQueryable<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression, bool flag = true);
		IDbQueryable<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression, bool flag = true);
		IDbQueryable<T> Take(int count, bool flag = true);
		IDbQueryable<T> Skip(int count, bool flag = true);
	}
}
