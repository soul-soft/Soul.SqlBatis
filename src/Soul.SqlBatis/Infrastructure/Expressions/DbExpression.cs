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

        public static DbExpression FromSqlExpression(RawSql token, DbExpressionType expressionType)
        {
            return new DbSqlExpression(token, expressionType);
        }

        public static DbExpression FromExpression(Expression expression, DbExpressionType expressionType)
        {
            return new DbExpression(expression, expressionType);
        }
    }

    internal class DbSqlExpression : DbExpression
    {
        public DbSqlExpression(RawSql token, DbExpressionType expressionType)
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
            ExpressionType = DbExpressionType.Set;
        }
    }
}
