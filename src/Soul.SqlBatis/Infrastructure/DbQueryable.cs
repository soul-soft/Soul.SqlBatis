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
				return;
			}
			var properties = param.GetType().GetProperties();
			foreach (var property in properties)
			{
				var name = property.Name;
				var value = property.GetValue(param);
				_parameters.Add(name, value);
			}
			return;
		}

		protected void AddExpression(DbExpression expression)
		{
			_expressions.Add(expression);
		}

		private string Build(Type type)
		{
			var builder = new DbExpressionBuilder(_context.Model, _parameters);

			var tokens = _expressions.Select(s => new
			{
				s.ExpressionType,
				Expression = builder.Build(s)
			}).ToList();

			var entityType = _context.Model.GetEntityType(type);
			var sb = new SqlBuilder();
			if (tokens.Any(a => a.ExpressionType == DbExpressionType.Where))
			{
				var wheres = tokens.Where(a => a.ExpressionType == DbExpressionType.Where);
				foreach (var item in wheres)
				{
					sb.Where(item.Expression);
				}
			}
			if (tokens.Any(a => a.ExpressionType == DbExpressionType.Having))
			{
				var havings = tokens.Where(a => a.ExpressionType == DbExpressionType.Having);
				foreach (var item in havings)
				{
					sb.Where(item.Expression);
				}
			}
			if (tokens.Any(a => a.ExpressionType == DbExpressionType.GroupBy))
			{
				var groups = tokens.Where(a => a.ExpressionType == DbExpressionType.GroupBy);
				foreach (var item in groups)
				{
					sb.Where(item.Expression);
				}
			}
			if (tokens.Any(a => a.ExpressionType == DbExpressionType.OrderBy))
			{
				var orders = tokens.Where(a => a.ExpressionType == DbExpressionType.OrderBy);
				foreach (var item in orders)
				{
					sb.Where(item.Expression);
				}
			}
			if (tokens.Any(a => a.ExpressionType == DbExpressionType.From))
			{
				var view = tokens.Where(a => a.ExpressionType == DbExpressionType.From).First().Expression;
				sb.From(view);
			}
			else
			{
				var view = entityType.TableName;
				sb.From(view);
			}
			if (tokens.Any(a => a.ExpressionType == DbExpressionType.Select))
			{
				var columns = tokens.Where(a => a.ExpressionType == DbExpressionType.Select).First().Expression;
				return sb.Select(columns);
			}
			else
			{
				var columns = entityType.Properties.Select(s => 
				{
					if (s.ColumnName == s.Member.Name)
					{
						return s.Member.Name;
					}
					return $"{s.ColumnName} AS {s.Member.Name}";
				});
				return sb.Select(columns);
			}
		}

		public List<T> ToList<T>()
		{
			var sql = Build(typeof(T));
			return _context.Query<T>(sql, _parameters).ToList();
		}

		public async Task<List<T>> ToListAsync<T>()
		{
			var sql = Build(typeof(T));
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
				AddExpression(item);
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
				AddExpression(DbExpression.FromExpression(expression, DbExpressionType.Where));
			return this;
		}
	}
}
