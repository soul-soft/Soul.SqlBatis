using System.Linq.Expressions;

namespace Soul.SqlBatis.Infrastructure
{
    public class DbSetExpression: DbExpression
    {
        public Expression Value { get; }
        public DbSetExpression(Expression expression,Expression value)
        {
            Value = value;
            Expression = expression;
            ExpressionType = DbExpressionType.Set;
        }
    }

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
