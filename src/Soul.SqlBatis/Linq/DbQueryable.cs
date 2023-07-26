using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Soul.SqlBatis.Expressions;

namespace Soul.SqlBatis
{
	public class DbQueryable<T> : DbQuery, IDbQueryable<T>
	{
		private readonly DbContext _context;

		public DbQueryable(DbContext context)
			: base(context)
		{
			_context = context;
		}

		public DbQueryable(DbContext context, List<DbSyntaxToken> expressions)
			: base(context, expressions)
		{
			_context = context;
		}

		public IDbQueryable<T> FromSql(DbSyntax sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbSyntaxToken.From(sql));
			}
			return this;
		}

		public IDbQueryable<T> GroupBy(DbSyntax sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbSyntaxToken.GroupBy(sql));
			}
			return this;
		}

		public IDbQueryable<T> GroupBy(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbSyntaxToken.GroupBy(expression));
			}
			return this;
		}

		public IDbQueryable<T> Having(DbSyntax sql, object param = null, bool flag = true)
		{
			if (flag)
			{
				AddParameter(param);
				AddExpression(DbSyntaxToken.Having(sql));
			}
			return this;
		}

		public IDbQueryable<T> Having(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbSyntaxToken.Having(expression));
			}
			return this;
		}

		public IDbQueryable<T> OrderBy(DbSyntax sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbSyntaxToken.OrderBy(sql));
			}
			return this;
		}

		public IDbQueryable<T> OrderBy(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbSyntaxToken.OrderBy(expression));
			}
			return this;
		}

		public IDbQueryable<T> OrderByDescending(DbSyntax sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbSyntaxToken.OrderBy(sql));
			}
			return this;
		}

		public IDbQueryable<T> OrderByDescending(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbSyntaxToken.OrderBy(expression));
			}
			return this;
		}

		public IDbQueryable<TResult> Select<TResult>(DbSyntax sql, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbSyntaxToken.Select(sql));
			}
			return new DbQueryable<TResult>(_context, Expressions);
		}

		public IDbQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbSyntaxToken.Select(expression));
			}
			return new DbQueryable<TResult>(_context, Expressions);
		}

		public IDbQueryable<T> Skip(int count, bool flag = true)
		{
			if (flag)
			{
				Expressions.Add(DbSyntaxToken.Skip(count));

			}
			return this;
		}

		public IDbQueryable<T> Take(int count, bool flag = true)
		{
			if (flag)
			{
				Expressions.Add(DbSyntaxToken.Take(count));
			}
			return this;
		}

		public IDbQueryable<T> Where(DbSyntax sql, object param = null, bool flag = true)
		{
			if (flag)
			{
				AddParameter(param);
				AddExpression(DbSyntaxToken.Where(sql));
			}
			return this;
		}

		public IDbQueryable<T> Where(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbSyntaxToken.Where(expression));
			}
			return this;
		}
	}
}
