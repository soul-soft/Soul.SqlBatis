using System.Linq.Expressions;

namespace Soul.SqlBatis.Expressions
{
	public class DbExpression
	{
		public Expression Expression { get; }

		public DbExpressionType ExpressionType { get; }

		private DbExpression(Expression expression, DbExpressionType expressionType)
		{
			Expression = expression;
			ExpressionType = expressionType;
		}

		public static DbExpression FromDbSql(DbSql sql, DbExpressionType expressionType)
		{
			return new DbExpression(Expression.Constant(sql), expressionType);
		}

		public static DbExpression FromExpression(Expression expression, DbExpressionType expressionType)
		{
			return new DbExpression(expression, expressionType);
		}
	}
}
