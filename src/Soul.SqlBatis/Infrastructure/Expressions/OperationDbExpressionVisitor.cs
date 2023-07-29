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
			if (node.Method.Name == nameof(DbFunc.Raw))
			{
				Visit(node.Arguments[0]);
			}
			else if (node.Method.Name == nameof(DbFunc.Contains))
			{
				if (!(node.Arguments[0] is MemberExpression))
				{
					throw new DbExpressionException("The first parameter must be a column");
				}
				Visit(node.Arguments[0]);
				SetSql(" LIKE ");
				var value = GetParameter(node.Arguments[1]);
				SetParameter(Expression.Constant($"%{value}%"));
			}
			else if (node.Method.Name == nameof(DbFunc.StartsWith))
			{
				if (!(node.Arguments[0] is MemberExpression))
				{
					throw new DbExpressionException("The first parameter must be a column");
				}
				Visit(node.Arguments[0]);
				SetSql(" LIKE ");
				var value = GetParameter(node.Arguments[1]);
				SetParameter(Expression.Constant($"{value}%"));
			}
			else if (node.Method.Name == nameof(DbFunc.StartsWith))
			{
				if (!(node.Arguments[0] is MemberExpression))
				{
					throw new DbExpressionException("The first parameter must be a column");
				}
				Visit(node.Arguments[0]);
				SetSql(" LIKE ");
				var value = GetParameter(node.Arguments[1]);
				SetParameter(Expression.Constant($"%{value}"));
			}
			else if (node.Method.Name == nameof(DbFunc.In))
			{
				if (!(node.Arguments[0] is MemberExpression))
				{
					throw new DbExpressionException("The first parameter must be a column");
				}
				Visit(node.Arguments[0]);
				SetSql(" IN ");
				SetParameter(node.Arguments[1]);
			}
			else if (node.Method.Name == nameof(DbFunc.IsNull))
			{
				if (!(node.Arguments[0] is MemberExpression))
				{
					throw new DbExpressionException("The first parameter must be a column");
				}
				Visit(node.Arguments[0]);
				SetBlankSpace();
				SetIsNULL();
			}
			else if (node.Method.Name == nameof(DbFunc.IsNotNull))
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
	}
}
