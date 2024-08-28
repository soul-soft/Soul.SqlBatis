using System.Collections.Generic;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Expressions
{
    public class SqlColumnExpressionParser : ExpressionVisitor
    {
        private readonly SqlExpressionContext _context;
        private readonly Dictionary<string, string> _items = new Dictionary<string, string>();

        protected SqlColumnExpressionParser(SqlExpressionContext context)
        {
            _context = context;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var member = _context.Model.Format(node.Member.Name);
            var column = SqlExpressionParser.Parse(_context, node);
            _items.Add(member, column);
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            for (int i = 0; i < node.Members.Count; i++)
            {
                var member = _context.Model.Format(node.Members[i].Name);
                var column = SqlExpressionParser.Parse(_context, node.Arguments[i]);
                _items.Add(member, column);
            }
            return node;
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            foreach (var item in node.Bindings)
            {
                var member = _context.Model.Format(item.Member.Name);
                var expression = ((MemberAssignment)item).Expression;
                var column = SqlExpressionParser.Parse(_context, expression);
                _items.Add(member, column);
            }
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var expr = SqlFunctionExpressionVisitor.Parse(_context, node);
            _items.Add("expr", expr);
            return node;
        }

        public static Dictionary<string, string> Parse(SqlExpressionContext context, Expression expression)
        {
            var parser = new SqlColumnExpressionParser(context);
            parser.Visit(expression);
            return parser._items;

        }
    }
}
