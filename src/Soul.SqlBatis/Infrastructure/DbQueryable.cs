using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Soul.SqlBatis.Expressions;
using Soul.SqlBatis.Linq;

namespace Soul.SqlBatis
{
	public class DbQueryable<T> : IDbQueryable<T>
	{
		private readonly DbContext _context;

		public DbQueryable(DbContext context)
		{
			_context = context;
		}

		public DbQueryable(DbContext context, List<DbExpression> expressions)
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

		public List<DbExpression> Expressions { get; } = new List<DbExpression>();

		private Dictionary<string, object> _parameters = new Dictionary<string, object>();

		public DbContext DbContext { get; }

		public DbQueryBuilder(DbContext context)
		{
			DbContext = context;
		}

		public DbQueryBuilder(DbContext context, List<DbExpression> expressions)
		{
			DbContext = context;
			Expressions = expressions;
		}

		protected void AddParameter(object param)
		{
			if (param == null)
			{
				return;
			}
			if (param is Dictionary<string, object> values)
			{
				foreach (var item in values)
				{
					_parameters.Add(item.Key, item.Value);
				}
			}
			var properties = param.GetType().GetProperties();
			foreach (var property in properties)
			{
				var name = property.Name;
				var value = property.GetValue(param);
				_parameters.Add(name, value);
			}
		}

		protected void AddExpression(DbExpression expression)
		{
			Expressions.Add(expression);
		}

		public DbCommand Build()
		{
			var engine = new DbExpressionEngine(DbContext.Model, _parameters);

			var list = Expressions.Select(s => new
			{
				Type = s.ExpressionType,
				Sql = engine.Build(s)
			}).ToList();


			return new DbCommand();
		}
	}
}
