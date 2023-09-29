using System;
using System.Linq.Expressions;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis
{
	public interface IDbQueryable<T>
	{
		IModel Model { get; }
		IDbQueryable<T> FromSql(SqlToken sql, DynamicParameters parameters = null);
		IDbQueryable<T> Set<TResult>(Expression<Func<T, TResult>> column, TResult value, bool flag = true);
		IDbQueryable<T> Set<TResult>(Expression<Func<T, TResult>> column, Expression<Func<T, TResult>> expression, bool flag = true);
		IDbQueryable<TResult> Select<TResult>(SqlToken sql, bool flag = true);
		IDbQueryable<T> Where(SqlToken sql, object param = null, bool flag = true);
		IDbQueryable<T> Having(SqlToken sql, object param = null, bool flag = true);
		IDbQueryable<T> GroupBy(SqlToken sql, bool flag = true);
		IDbQueryable<T> OrderBy(SqlToken sql, bool flag = true);
		IDbQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool flag = true);
		IDbQueryable<T> Where(Expression<Func<T, bool>> expression, bool flag = true);
		IDbQueryable<T> Having(Expression<Func<T, bool>> expression, bool flag = true);
		IDbQueryable<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression, bool flag = true);
		IDbQueryable<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression, bool flag = true);
		IDbQueryable<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression, bool flag = true);
		IDbQueryable<T> Take(int count, bool flag = true);
		IDbQueryable<T> Skip(int count, bool flag = true);
		IDbQueryable<T> Clone();
		IDbQueryable<TResult> Clone<TResult>();
	}
}
