using System.Collections.Generic;
using System.Linq.Expressions;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Expressions
{
    internal class OperationDbExpressionVisitor : DbExpressionVisitor
    {
        public OperationDbExpressionVisitor(Model model, Dictionary<string, object> parameters)
            : base(model, parameters)
        {

        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == nameof(DbOperations.Raw))
            {
                Visit(node.Arguments[0]);
            }
            else if (node.Method.Name == nameof(DbOperations.Contains))
            {
                Expression columnExpression = null;
                Expression valueExpression = null;
                if (IsParameterExpression(node.Arguments[0]))
                {
                    columnExpression = node.Arguments[0];
                    valueExpression = node.Arguments[1];
                }
                else
                {
                    columnExpression = node.Arguments[1];
                    valueExpression = node.Arguments[0];
                }
                Visit(columnExpression);
                SetSql(" LIKE ");
                var value = GetParameter(valueExpression);
                SetParameter(Expression.Constant($"%{value}%"));
            }
            else if (node.Method.Name == nameof(DbOperations.StartsWith))
            {
                Expression columnExpression = null;
                Expression valueExpression = null;
                if (IsParameterExpression(node.Arguments[0]))
                {
                    columnExpression = node.Arguments[0];
                    valueExpression = node.Arguments[1];
                }
                else
                {
                    columnExpression = node.Arguments[1];
                    valueExpression = node.Arguments[0];
                }
                Visit(columnExpression);
                SetSql(" LIKE ");
                var value = GetParameter(valueExpression);
                SetParameter(Expression.Constant($"{value}%"));
            }
            else if (node.Method.Name == nameof(DbOperations.StartsWith))
            {
                Expression columnExpression = null;
                Expression valueExpression = null;
                if (IsParameterExpression(node.Arguments[0]))
                {
                    columnExpression = node.Arguments[0];
                    valueExpression = node.Arguments[1];
                }
                else
                {
                    columnExpression = node.Arguments[1];
                    valueExpression = node.Arguments[0];
                }
                Visit(columnExpression);
                SetSql(" LIKE ");
                var value = GetParameter(valueExpression);
                SetParameter(Expression.Constant($"%{value}"));
            }
            else if (node.Method.Name == nameof(DbOperations.In))
            {
                if (!(node.Arguments[0] is MemberExpression))
                {
                    throw new DbExpressionException("The first parameter must be a column");
                }
                Visit(node.Arguments[0]);
                SetSql(" IN ");
                SetParameter(node.Arguments[1]);
            }
            else if (node.Method.Name == nameof(DbOperations.IsNull))
            {
                if (!(node.Arguments[0] is MemberExpression))
                {
                    throw new DbExpressionException("The first parameter must be a column");
                }
                Visit(node.Arguments[0]);
                SetBlankSpace();
                SetIsNULL();
            }
            else if (node.Method.Name == nameof(DbOperations.IsNotNull))
            {
                if (!(node.Arguments[0] is MemberExpression))
                {
                    throw new DbExpressionException("The first parameter must be a column");
                }
                Visit(node.Arguments[0]);
                SetBlankSpace();
                SetIsNotNULL();
            }
            return node;
        }

        private bool IsParameterExpression(Expression expression)
        {
            if (!(expression is MemberExpression))
            {
                return false;
            }
            var member = expression as MemberExpression;
            if (member.Expression != null && member.Expression.NodeType == ExpressionType.Parameter)
            {
                return true;
            }
            return false;
        }
    }
}
