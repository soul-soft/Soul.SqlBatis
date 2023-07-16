using System.Drawing;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Expressions
{
    public class SqlExpression
    {
        public System.Linq.Expressions.Expression Expression { get; }
        public SqlExpressionType ExpressionType { get; }

        public SqlExpression(System.Linq.Expressions.Expression expression, SqlExpressionType expressionType)
        {
            Expression = expression;
            ExpressionType = expressionType;
        }

        public static SqlExpression From(string expression)
        {
            return From(System.Linq.Expressions.Expression.Constant(expression));
        }

        public static SqlExpression From(System.Linq.Expressions.Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.From);
        }

        public static SqlExpression Select(string expression)
        {
            return Select(System.Linq.Expressions.Expression.Constant(expression));
        }

        public static SqlExpression Select(System.Linq.Expressions.Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.Select);
        }

        public static SqlExpression Where(string expression)
        {
            return Where(System.Linq.Expressions.Expression.Constant(expression));
        }

        public static SqlExpression Where(System.Linq.Expressions.Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.Where);
        }

        public static SqlExpression GroupBy(string expression)
        {
            return GroupBy(expression);
        }

        public static SqlExpression GroupBy(System.Linq.Expressions.Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.GroupBy);
        }

        public static SqlExpression Having(string expression)
        {
            return Having(System.Linq.Expressions.Expression.Constant(expression));
        }

        public static SqlExpression Having(System.Linq.Expressions.Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.Having);
        }

        public static SqlExpression OrderBy(string expression)
        {
            return OrderBy(System.Linq.Expressions.Expression.Constant(expression));
        }

        public static SqlExpression OrderBy(System.Linq.Expressions.Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.OrderBy);
        }

        public static SqlExpression OrderByDescending(System.Linq.Expressions.Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.OrderByDescending);
        }

        public static SqlExpression Take(int count)
        {
            return new SqlExpression(System.Linq.Expressions.Expression.Constant(count), SqlExpressionType.Take);
        }

        public static SqlExpression Skip(int count)
        {
            return new SqlExpression(System.Linq.Expressions.Expression.Constant(count), SqlExpressionType.Skip);
        }

    }
}
