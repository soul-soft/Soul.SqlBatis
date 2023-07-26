using System.Collections.Generic;
using Soul.SqlBatis.Expressions;

namespace Soul.SqlBatis
{
	public abstract class DbQuery
	{
		public List<DbSyntaxToken> Expressions { get; } = new List<DbSyntaxToken>();

		private Dictionary<string, object> _parameters = new Dictionary<string, object>();

		public DbContext DbContext { get; }

		public DbQuery(DbContext context)
		{
			DbContext = context;
		}

		public DbQuery(DbContext context, List<DbSyntaxToken> expressions)
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

		protected void AddExpression(DbSyntaxToken expression)
		{
			Expressions.Add(expression);
		}

		public DbCommand Build()
		{
			var tokens = new Dictionary<DbSyntaxTokenType, string>();
			return new DbCommand();
		}
	}
}
