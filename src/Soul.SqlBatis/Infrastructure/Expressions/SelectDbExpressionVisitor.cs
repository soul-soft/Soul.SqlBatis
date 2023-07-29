using System.Collections.Generic;
using System.Linq.Expressions;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Expressions
{
    internal class SelectDbExpressionVisitor : DbExpressionVisitor
    {
        private readonly List<string> _columns = new List<string>();

        public SelectDbExpressionVisitor(Model model, Dictionary<string, object> parameters)
            : base(model, parameters)
        {
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is ParameterExpression)
            {
                var column = GetColumn(node.Member);
                AddColumn(column, node.Member.Name);
            }
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            foreach (var item in node.Members)
            {
                var member = item.Name;
                var expression = node.Arguments[node.Members.IndexOf(item)];
                var column = new DbExpressionVisitor(Model, Parameters).Build(expression);
                AddColumn(column, member);
            }
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var column = new DbExpressionVisitor(Model, Parameters).Build(node);
            AddColumn(column);
            return node;
        }

        private void AddColumn(string column)
        {
            var member = $"c{_columns.Count}";
            _columns.Add($"{column} AS {member}");
        }

        private void AddColumn(string column, string member)
        {
            if (column.Equals(member))
            {
                _columns.Add($"{column}");
            }
            else
            {
                _columns.Add($"{column} AS {member}");
            }
        }

        public override string Build(Expression expression)
        {
            Visit(expression);
            return Build(string.Join(", ", _columns));
        }
    }
}
