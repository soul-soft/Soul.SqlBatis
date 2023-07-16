using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Soul.SqlBatis.Expressions
{
	public abstract class SqlExpressionVisitor : ExpressionVisitor
	{
		protected StringBuilder Buffer = new StringBuilder();
		private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

		protected SqlExpressionVisitor(Dictionary<string, object> parameters)
		{
			_parameters = parameters;
		}

		public abstract string Build(Expression expression, Dictionary<string, object> parameters);
	}
}
