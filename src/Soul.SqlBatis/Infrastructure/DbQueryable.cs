using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Soul.SqlBatis.Expressions;

namespace Soul.SqlBatis
{
	public class DbQueryable<T> : DbQueryBuilder, IDbQueryable<T>
	{
		private readonly DbContext _context;

		public DbQueryable(DbContext context)
			: base(context)
		{
			_context = context;
		}

		public DbQueryable(DbContext context, List<DbExpression> expressions)
			: base(context, expressions)
		{
			_context = context;
		}

		public IDbQueryable<T> FromSql(DbSql sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbExpression.From(sql));
			}
			return this;
		}

		public IDbQueryable<T> GroupBy(DbSql sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbExpression.GroupBy(sql));
			}
			return this;
		}

		public IDbQueryable<T> GroupBy(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbExpression.GroupBy(expression));
			}
			return this;
		}

		public IDbQueryable<T> Having(DbSql sql, object param = null, bool flag = true)
		{
			if (flag)
			{
				AddParameter(param);
				AddExpression(DbExpression.Having(sql));
			}
			return this;
		}

		public IDbQueryable<T> Having(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbExpression.Having(expression));
			}
			return this;
		}

		public IDbQueryable<T> OrderBy(DbSql sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbExpression.OrderBy(sql));
			}
			return this;
		}

		public IDbQueryable<T> OrderBy(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbExpression.OrderBy(expression));
			}
			return this;
		}

		public IDbQueryable<T> OrderByDescending(DbSql sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbExpression.OrderBy(sql));
			}
			return this;
		}

		public IDbQueryable<T> OrderByDescending(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbExpression.OrderBy(expression));
			}
			return this;
		}

		public IDbQueryable<TResult> Select<TResult>(DbSql sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbExpression.Select(sql));
			}
			return new DbQueryable<TResult>(_context, Expressions);
		}

		public IDbQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbExpression.Select(expression));
			}
			return new DbQueryable<TResult>(_context, Expressions);
		}

		public IDbQueryable<T> Skip(int count, bool flag = true)
		{
			if (flag)
			{
				Expressions.Add(DbExpression.Skip(count));

			}
			return this;
		}

		public IDbQueryable<T> Take(int count, bool flag = true)
		{
			if (flag)
			{
				Expressions.Add(DbExpression.Take(count));
			}
			return this;
		}

		public IDbQueryable<T> Where(DbSql sql, object param = null, bool flag = true)
		{
			if (flag)
			{
				AddParameter(param);
				AddExpression(DbExpression.Where(sql));
			}
			return this;
		}

		public IDbQueryable<T> Where(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbExpression.Where(expression));
			}
			return this;
		}
	}
}
