using System.Linq.Expressions;

namespace Soul.SqlBatis.Infrastructure
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

        public static DbExpression FromSqlExpression(DbSqlExpression sql, DbExpressionType expressionType)
        {
            return new DbExpression(Expression.Constant(sql), expressionType);
        }

        public static DbExpression FromLambdaExpression(Expression expression, DbExpressionType expressionType)
        {
            return new DbExpression(expression, expressionType);
        }
    }
}
