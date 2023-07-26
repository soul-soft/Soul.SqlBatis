using System.Collections.Generic;
using System.Linq.Expressions;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Expressions
{
    public class DbExpression
    {
        public Expression Expression { get; }
        public DbExpressionType ExpressionType { get; }

        public DbExpression(Expression expression, DbExpressionType expressionType)
        {
            Expression = expression;
            ExpressionType = expressionType;
        }

        public static DbExpression From(DbSyntax sql)
        {
            return From(Expression.Constant(sql));
        }

        public static DbExpression From(Expression expression)
        {
            return new DbExpression(expression, DbExpressionType.From);
        }

        public static DbExpression Select(DbSyntax sql)
        {
            return Select(Expression.Constant(sql));
        }

        public static DbExpression Select(Expression expression)
        {
            return new DbExpression(expression, DbExpressionType.Select);
        }

        public static DbExpression Where(DbSyntax sql)
        {
            return Where(Expression.Constant(sql));
        }

        public static DbExpression Where(Expression expression)
        {
            return new DbExpression(expression, DbExpressionType.Where);
        }

        public static DbExpression GroupBy(DbSyntax sql)
        {
            return GroupBy(sql);
        }

        public static DbExpression GroupBy(Expression expression)
        {
            return new DbExpression(expression, DbExpressionType.GroupBy);
        }

        public static DbExpression Having(DbSyntax sql)
        {
            return Having(Expression.Constant(sql));
        }

        public static DbExpression Having(Expression expression)
        {
            return new DbExpression(expression, DbExpressionType.Having);
        }

        public static DbExpression OrderBy(DbSyntax sql)
        {
            return OrderBy(Expression.Constant(sql));
        }

        public static DbExpression OrderBy(Expression expression)
        {
            return new DbExpression(expression, DbExpressionType.OrderBy);
        }

        public static DbExpression OrderByDescending(Expression expression)
        {
            return new DbExpression(expression, DbExpressionType.OrderByDescending);
        }

        public static DbExpression Take(int count)
        {
            return new DbExpression(Expression.Constant(count), DbExpressionType.Take);
        }

        public static DbExpression Skip(int count)
        {
            return new DbExpression(Expression.Constant(count), DbExpressionType.Skip);
        }
    }
}
