using System.Linq.Expressions;

namespace Soul.SqlBatis.Infrastructure
{
    public class DbExpression
    {
        public Expression Expression { get; protected set; }

        public DbExpressionToken ExpressionType { get; protected set; }

        protected DbExpression()
        {

        }

        private DbExpression(Expression expression, DbExpressionToken expressionType)
        {
            Expression = expression;
            ExpressionType = expressionType;
        }

        public static DbExpression FromSetExpression(Expression expression, Expression value)
        {
            return new DbSetExpression(expression, value);
        }

        public static DbExpression FromSqlExpression(RawSql token, DbExpressionToken expressionType)
        {
            return new DbSqlExpression(token, expressionType);
        }

        public static DbExpression FromExpression(Expression expression, DbExpressionToken expressionType)
        {
            return new DbExpression(expression, expressionType);
        }
    }

    internal class DbSqlExpression : DbExpression
    {
        public DbSqlExpression(RawSql token, DbExpressionToken expressionType)
        {
            Expression = Expression.Constant(token, typeof(RawSql));
            ExpressionType = expressionType;
        }
    }

    internal class DbSetExpression : DbExpression
    {
        public Expression Value { get; }

        public DbSetExpression(Expression expression, Expression value)
        {
            Value = value;
            Expression = expression;
            ExpressionType = DbExpressionToken.Set;
        }
    }
}
