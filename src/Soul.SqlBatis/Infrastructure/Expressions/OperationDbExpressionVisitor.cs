using System.Collections.Generic;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Infrastructure
{
    internal class OperationDbExpressionVisitor : DbExpressionVisitor
    {
        public OperationDbExpressionVisitor(Model model, DynamicParameters parameters)
            : base(model, parameters)
        {

        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == nameof(DbOperations.Raw))
            {
                var constantExpression = node.Arguments[0] as ConstantExpression;
                SetSql(constantExpression.Value.ToString());
            }
            else if (node.Method.Name == nameof(DbOperations.Like))
            {
                Visit(node.Arguments[0]);
                SetBlank();
                SetLike();
                SetBlank();
                Visit(node.Arguments[1]);
            }
            else if (node.Method.Name == nameof(DbOperations.Contains))
            {
                Expression columnExpression = null;
                Expression valueExpression = null;
                if (IsParameterMemberExpression(node.Arguments[0]))
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
                SetBlank();
                SetLike();
                SetBlank();
                var value = GetParameter(valueExpression);
                SetParameter(Expression.Constant($"%{value}%"));
            }
            else if (node.Method.Name == nameof(DbOperations.StartsWith))
            {
                Expression columnExpression = null;
                Expression valueExpression = null;
                if (IsParameterMemberExpression(node.Arguments[0]))
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
                SetBlank();
                SetLike();
                SetBlank();
                var value = GetParameter(valueExpression);
                SetParameter(Expression.Constant($"{value}%"));
            }
            else if (node.Method.Name == nameof(DbOperations.StartsWith))
            {
                Expression columnExpression = null;
                Expression valueExpression = null;
                if (IsParameterMemberExpression(node.Arguments[0]))
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
                SetBlank();
                SetLike();
                SetBlank();
                var value = GetParameter(valueExpression);
                SetParameter(Expression.Constant($"%{value}"));
            }
            else if (node.Method.Name == nameof(DbOperations.In))
            {
                Visit(node.Arguments[0]);
                SetBlank();
                SetIn();
                SetBlank();
                SetParameter(node.Arguments[1]);
            }
            else if (node.Method.Name == nameof(DbOperations.IsNull))
            {
                Visit(node.Arguments[0]);
                SetBlank();
                SetIsNULL();
            }
            else if (node.Method.Name == nameof(DbOperations.IsNotNull))
            {
                Visit(node.Arguments[0]);
                SetBlank();
                SetIsNotNULL();
            }
            else if (node.Method.Name == nameof(DbOperations.Between))
            {
                Visit(node.Arguments[0]);
                SetBlank();
                SetBetween();
                SetBlank();
                Visit(node.Arguments[1]);
                SetBlank();
                SetBinaryType(ExpressionType.AndAlso);
                SetBlank();
                Visit(node.Arguments[2]);
            }
            return node;
        }
    }
}
