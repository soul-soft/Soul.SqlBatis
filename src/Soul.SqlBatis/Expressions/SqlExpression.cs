using System.Drawing;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Expressions
{
    public class SqlExpression
    {
        public Expression Expression { get; }
        public SqlExpressionType ExpressionType { get; }

        public SqlExpression(Expression expression, SqlExpressionType expressionType)
        {
            Expression = expression;
            ExpressionType = expressionType;
        }

        public static SqlExpression From(DbSql sql)
        {
            return From(Expression.Constant(sql));
        }

        public static SqlExpression From(Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.From);
        }

        public static SqlExpression Select(DbSql sql)
        {
            return Select(Expression.Constant(sql));
        }

        public static SqlExpression Select(Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.Select);
        }

        public static SqlExpression Where(DbSql sql)
        {
            return Where(Expression.Constant(sql));
        }

        public static SqlExpression Where(Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.Where);
        }

        public static SqlExpression GroupBy(DbSql sql)
        {
            return GroupBy(sql);
        }

        public static SqlExpression GroupBy(Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.GroupBy);
        }

        public static SqlExpression Having(DbSql sql)
        {
            return Having(Expression.Constant(sql));
        }

        public static SqlExpression Having(Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.Having);
        }

        public static SqlExpression OrderBy(DbSql sql)
        {
            return OrderBy(Expression.Constant(sql));
        }

        public static SqlExpression OrderBy(Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.OrderBy);
        }

        public static SqlExpression OrderByDescending(Expression expression)
        {
            return new SqlExpression(expression, SqlExpressionType.OrderByDescending);
        }

        public static SqlExpression Take(int count)
        {
            return new SqlExpression(Expression.Constant(count), SqlExpressionType.Take);
        }

        public static SqlExpression Skip(int count)
        {
            return new SqlExpression(Expression.Constant(count), SqlExpressionType.Skip);
        }

    }
}
