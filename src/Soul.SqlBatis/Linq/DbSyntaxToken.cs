using System.Linq.Expressions;

namespace Soul.SqlBatis.Expressions
{
    public class DbSyntaxToken
    {
        public Expression Expression { get; }
        public DbSyntaxTokenType TokenType { get; }

        public DbSyntaxToken(Expression expression, DbSyntaxTokenType expressionType)
        {
            Expression = expression;
            TokenType = expressionType;
        }

        public static DbSyntaxToken From(DbSyntax sql)
        {
            return From(Expression.Constant(sql));
        }

        public static DbSyntaxToken From(Expression expression)
        {
            return new DbSyntaxToken(expression, DbSyntaxTokenType.From);
        }

        public static DbSyntaxToken Select(DbSyntax sql)
        {
            return Select(Expression.Constant(sql));
        }

        public static DbSyntaxToken Select(Expression expression)
        {
            return new DbSyntaxToken(expression, DbSyntaxTokenType.Select);
        }

        public static DbSyntaxToken Where(DbSyntax sql)
        {
            return Where(Expression.Constant(sql));
        }

        public static DbSyntaxToken Where(Expression expression)
        {
            return new DbSyntaxToken(expression, DbSyntaxTokenType.Where);
        }

        public static DbSyntaxToken GroupBy(DbSyntax sql)
        {
            return GroupBy(sql);
        }

        public static DbSyntaxToken GroupBy(Expression expression)
        {
            return new DbSyntaxToken(expression, DbSyntaxTokenType.GroupBy);
        }

        public static DbSyntaxToken Having(DbSyntax sql)
        {
            return Having(Expression.Constant(sql));
        }

        public static DbSyntaxToken Having(Expression expression)
        {
            return new DbSyntaxToken(expression, DbSyntaxTokenType.Having);
        }

        public static DbSyntaxToken OrderBy(DbSyntax sql)
        {
            return OrderBy(Expression.Constant(sql));
        }

        public static DbSyntaxToken OrderBy(Expression expression)
        {
            return new DbSyntaxToken(expression, DbSyntaxTokenType.OrderBy);
        }

        public static DbSyntaxToken OrderByDescending(Expression expression)
        {
            return new DbSyntaxToken(expression, DbSyntaxTokenType.OrderByDescending);
        }

        public static DbSyntaxToken Take(int count)
        {
            return new DbSyntaxToken(Expression.Constant(count), DbSyntaxTokenType.Take);
        }

        public static DbSyntaxToken Skip(int count)
        {
            return new DbSyntaxToken(Expression.Constant(count), DbSyntaxTokenType.Skip);
        }

    }
}
