using System.Linq.Expressions;

namespace Soul.SqlBatis.Infrastructure
{
    public class DbExpression
    {
        public Expression Expression { get; protected set; }

        public DbExpressionType ExpressionType { get; protected set; }

        protected DbExpression()
        {

        }

        private DbExpression(Expression expression, DbExpressionType expressionType)
        {
            Expression = expression;
            ExpressionType = expressionType;
        }

        public static DbExpression FromSetExpression(Expression expression, Expression value)
        {
            return new DbSetExpression(expression, value);
        }

        public static DbExpression FromSqlExpression(SqlToken sql, DbExpressionType expressionType)
        {
            return new DbSqlExpression(sql, expressionType);
        }

        public static DbExpression FromLambdaExpression(Expression expression, DbExpressionType expressionType)
        {
            return new DbExpression(expression, expressionType);
        }
    }

    public class DbSqlExpression : DbExpression
    {
        public DbSqlExpression(SqlToken token, DbExpressionType expressionType)
        {
            Expression = Expression.Constant(token);
            ExpressionType = expressionType;
        }
    }

    public class DbSetExpression : DbExpression
    {
        public Expression Value { get; }
        public DbSetExpression(Expression expression, Expression value)
        {
            Value = value;
            Expression = expression;
            ExpressionType = DbExpressionType.Set;
        }
    }
}
