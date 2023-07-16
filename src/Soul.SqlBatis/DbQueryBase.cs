using System;
using System.Collections.Generic;
using System.Data.Common;
using Soul.SqlBatis.Expressions;

namespace Soul.SqlBatis
{
	public abstract class DbQueryBase
	{
		private List<SqlExpression> _expressions = new List<SqlExpression>();

		private Dictionary<string, object> _parameters = new Dictionary<string, object>();

		public DbContext DbContext { get; }

		public DbQueryBase(DbContext context)
		{
			DbContext = context;
		}

		public DbQueryBase(DbContext context, List<SqlExpression> expressions)
		{
			DbContext = context;
			_expressions = expressions;
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
			_expressions.Add(expression);
		}

		protected List<SqlExpression> GetExpressions()
		{
			return _expressions; 
		}

		public DbCommand Build()
		{
			return new DbCommand();
		}
	}
}
