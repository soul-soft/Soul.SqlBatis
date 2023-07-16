using System.Collections.Generic;
using Soul.SqlBatis.Expressions;

namespace Soul.SqlBatis
{
	public abstract class DbQueryBase
	{
		public List<SqlExpression> Expressions { get; } = new List<SqlExpression>();

		private Dictionary<string, object> _parameters = new Dictionary<string, object>();

		public DbContext DbContext { get; }

		public DbQueryBase(DbContext context)
		{
			DbContext = context;
		}

		public DbQueryBase(DbContext context, List<SqlExpression> expressions)
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

		protected void AddExpression(SqlExpression expression)
		{
			Expressions.Add(expression);
		}

		public DbCommand Build()
		{
			return new DbCommand();
		}
	}
}
