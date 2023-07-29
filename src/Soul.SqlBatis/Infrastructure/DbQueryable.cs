using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using Soul.SqlBatis.Expressions;
using Soul.SqlBatis.Linq;

namespace Soul.SqlBatis
{
	public class DbQueryable<T> : IDbQueryable<T>
	{
		private readonly DbContext _context;

		private readonly List<DbExpression> _expressions = new List<DbExpression>();

		private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

		public DbQueryable(DbContext context)
		{
			_context = context;
		}

		public DbQueryable(DbContext context, List<DbExpression> expressions)
		{
			_context = context;
			_expressions = expressions;
		}

		public IDbQueryable<T> FromSql(DbSql sql, bool flag = true)
		{
			if (flag)
				AddExpression(DbExpression.FromDbSql(sql, DbExpressionType.From));
			return this;
		}

		public IDbQueryable<T> GroupBy(DbSql sql, bool flag = true)
		{
			if (flag)
				AddExpression(DbExpression.FromDbSql(sql, DbExpressionType.GroupBy));
			return this;
		}

		public IDbQueryable<T> GroupBy(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
				AddExpression(DbExpression.FromExpression(expression, DbExpressionType.GroupBy));
			return this;
		}

		public IDbQueryable<T> Having(DbSql sql, object param = null, bool flag = true)
		{
			if (flag)
			{
				AddParameters(param);
				AddExpression(DbExpression.FromDbSql(sql, DbExpressionType.Having));
			}
			return this;
		}

		public IDbQueryable<T> Having(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
				AddExpression(DbExpression.FromExpression(expression, DbExpressionType.Having));
			return this;
		}

		public IDbQueryable<T> OrderBy(DbSql sql, bool flag = true)
		{
			if (flag)
				AddExpression(DbExpression.FromDbSql(sql, DbExpressionType.OrderBy));
			return this;
		}

		public IDbQueryable<T> OrderBy(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
				AddExpression(DbExpression.FromExpression(expression, DbExpressionType.OrderBy));
			return this;
		}

		public IDbQueryable<T> OrderByDescending(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
				AddExpression(DbExpression.FromExpression(expression, DbExpressionType.OrderByDescending));
			return this;
		}

		public IDbQueryable<TResult> Select<TResult>(DbSql sql, bool flag = true)
		{
			if (flag)
				AddExpression(DbExpression.FromDbSql(sql, DbExpressionType.Select));
			return Clone<TResult>();
		}

		public IDbQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
		{
			if (flag)
				AddExpression(DbExpression.FromExpression(expression, DbExpressionType.Select));
			return Clone<TResult>();
		}

		public IDbQueryable<T> Skip(int count, bool flag = true)
		{
			if (flag)
				AddExpression(DbExpression.FromDbSql(count.ToString(), DbExpressionType.Skip));
			return this;
		}

		public IDbQueryable<T> Take(int count, bool flag = true)
		{
			if (flag)
				AddExpression(DbExpression.FromDbSql(count.ToString(), DbExpressionType.Take));
			return this;
		}

		public IDbQueryable<T> Where(DbSql sql, object param = null, bool flag = true)
		{
			if (flag)
			{
				AddParameters(param);
				AddExpression(DbExpression.FromDbSql(sql, DbExpressionType.Where));
			}
			return this;
		}

		public IDbQueryable<T> Where(Expression<Func<T, bool>> expression, bool flag = true)
		{
			if (flag)
			{
				AddExpression(DbExpression.FromExpression(expression, DbExpressionType.Where));
			}
			return this;
		}

		private void AddParameters(object param)
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

		private void AddExpression(DbExpression expression)
		{
			_expressions.Add(expression);
		}

		public DbCommand Build()
		{
			var engine = new DbExpressionEngine(_context.Model, _parameters);

			var list = _expressions.Select(s => new
			{
				Type = s.ExpressionType,
				Sql = engine.Build(s)
			}).ToList();

			return new DbCommand();
		}

		public IDbQueryable<T> Clone()
		{
			return new DbQueryable<T>(_context, _expressions);
		}

		public IDbQueryable<TResult> Clone<TResult>()
		{
			return new DbQueryable<TResult>(_context, _expressions);
		}
	}
}
