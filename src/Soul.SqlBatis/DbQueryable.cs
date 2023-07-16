﻿using System;
using System.Collections.Generic;
using System.Data.Common;
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

		public IDbQueryable<T> FromSql(string sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.From(sql));
			}
			return this;
		}

		public IDbQueryable<T> GroupBy(string sql, bool flag = true)
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

		public IDbQueryable<T> Having(string sql, object param = null, bool flag = true)
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

		public IDbQueryable<T> OrderBy(string sql, bool flag = true)
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

		public IDbQueryable<T> OrderByDescending(string sql, bool flag = true)
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

		public IDbQueryable<TResult> Select<TResult>(string sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.Select(sql));
			}
			return new DbQueryable<TResult>(_context, GetExpressions());
		}

		public IDbQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(SqlExpression.Select(expression));
			}
			return new DbQueryable<TResult>(_context, GetExpressions());
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

		public IDbQueryable<T> Where(string sql, object param = null, bool flag = true)
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
