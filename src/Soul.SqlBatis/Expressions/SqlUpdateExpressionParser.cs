using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Soul.SqlBatis.Expressions
{
    public class SqlUpdateExpressionParser : ExpressionVisitor
    {
        private readonly SqlExpressionContext _context;
        private readonly List<string> _setters = new List<string>();

        protected SqlUpdateExpressionParser(SqlExpressionContext context)
        {
            _context = context;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.ReturnType.IsGenericType
               && node.Method.ReturnType.GenericTypeArguments.Length == 1
               && node.Method.DeclaringType == typeof(DbUpdateQueryable<>).MakeGenericType(node.Method.ReturnType.GenericTypeArguments[0]))
            {
                if (node.Object.NodeType == ExpressionType.Call)
                {
                    Visit(node.Object);
                }
                var member = SqlExpressionParser.Parse(_context, node.Arguments[0]);
                var value = SqlExpressionParser.Parse(_context, node.Arguments[1]);
                _setters.Add($"{member} = {value}");
            }
            return node;
        }

        public static List<string> Parse(SqlExpressionContext context, Expression expression)
        {
            var parser = new SqlUpdateExpressionParser(context);
            parser.Visit(expression);
            return parser._setters;
        }
    }
}
