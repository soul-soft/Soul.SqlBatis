using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Soul.SqlBatis.Expressions;

namespace Soul.SqlBatis
{
	public class DbQueryable<T> : DbQueryBase, IDbQueryable<T>
	{
		private readonly DbContext _context;

		public DbQueryable(DbContext context)
			: base(context)
		{
			_context = context;
		}

		public DbQueryable(DbContext context, List<SqlExpression> expressions)
			: base(context, expressions)
		{
			_context = context;
		}

		public IDbQueryable<T> FromSql(DbSql sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.From(sql));
			}
			return this;
		}

		public IDbQueryable<T> GroupBy(DbSql sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.GroupBy(sql));
			}
			return this;
		}

		public IDbQueryable<T> GroupBy(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.GroupBy(expression));
			}
			return this;
		}

		public IDbQueryable<T> Having(DbSql sql, object param = null, bool flag = true)
		{
			if (flag)
			{
				AddParameter(param);
				AddExpression(SqlExpression.Having(sql));
			}
			return this;
		}

		public IDbQueryable<T> Having(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.Having(expression));
			}
			return this;
		}

		public IDbQueryable<T> OrderBy(DbSql sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.OrderBy(sql));
			}
			return this;
		}

		public IDbQueryable<T> OrderBy(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.OrderBy(expression));
			}
			return this;
		}

		public IDbQueryable<T> OrderByDescending(DbSql sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.OrderBy(sql));
			}
			return this;
		}

		public IDbQueryable<T> OrderByDescending(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.OrderBy(expression));
			}
			return this;
		}

		public IDbQueryable<TResult> Select<TResult>(DbSql sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.Select(sql));
			}
			return new DbQueryable<TResult>(_context, Expressions);
		}

		public IDbQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.Select(expression));
			}
			return new DbQueryable<TResult>(_context, Expressions);
		}

		public IDbQueryable<T> Skip(int count, bool flag = true)
		{
			if (flag)
			{
				Expressions.Add(SqlExpression.Skip(count));

			}
			return this;
		}

		public IDbQueryable<T> Take(int count, bool flag = true)
		{
			if (flag)
			{
				Expressions.Add(SqlExpression.Take(count));
			}
			return this;
		}

		public IDbQueryable<T> Where(DbSql sql, object param = null, bool flag = true)
		{
			if (flag)
			{
				AddParameter(param);
				AddExpression(SqlExpression.Where(sql));
			}
			return this;
		}

		public IDbQueryable<T> Where(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.Where(expression));
			}
			return this;
		}
	}
}
