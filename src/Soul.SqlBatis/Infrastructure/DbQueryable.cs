using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Soul.SqlBatis.Expressions;
using Soul.SqlBatis.Linq;

namespace Soul.SqlBatis
{
	public abstract class DbQueryable
	{
		private readonly DbContext _context;

		private readonly List<DbExpression> _expressions = new List<DbExpression>();

		private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

		protected DbContext DbContext => _context;

		protected IReadOnlyCollection<DbExpression> Expressions => _expressions;

		protected IReadOnlyDictionary<string, object> Parameters => _parameters;

		public DbQueryable(DbContext context)
		{
			_context = context;
		}

		protected void AddParameters(object param)
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
			_expressions.Add(expression);
		}

		private string Build()
		{
			var engine = new DbExpressionEngine(_context.Model, _parameters);

			var list = _expressions.Select(s => new
			{
				Type = s.ExpressionType,
				Sql = engine.Build(s)
			}).ToList();

			return string.Empty;
		}

		public List<T> ToList<T>()
		{
			var sql = Build();
			return _context.Query<T>(sql, _parameters).ToList();
		}

		public async Task<List<T>> ToListAsync<T>()
		{
			var sql = Build();
			var list = await _context.QueryAsync<T>(sql, _parameters);
			return list.ToList();
		}
	}

	public class DbQueryable<T> : DbQueryable, IDbQueryable<T>
	{
		public DbQueryable(DbContext context)
			: base(context)
		{

		}

		private DbQueryable(DbContext context, IEnumerable<DbExpression> expressions, object parameters)
			: base(context)
		{
			foreach (var item in expressions)
			{
				AddExpression(item);
			}
			AddParameters(parameters);
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
			return new DbQueryable<TResult>(DbContext, Expressions, Parameters);
		}

		public IDbQueryable<TResult> Select<TResult>(Expression<Func<T, TResult>> expression, bool flag = true)
		{
			if (flag)
				AddExpression(DbExpression.FromExpression(expression, DbExpressionType.Select));
			return new DbQueryable<TResult>(DbContext, Expressions, Parameters);
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
	}
}
