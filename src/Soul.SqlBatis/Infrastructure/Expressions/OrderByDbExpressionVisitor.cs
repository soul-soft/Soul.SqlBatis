using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Infrastructure
{
	internal class OrderByDbExpressionVisitor : DbExpressionVisitor
	{
		private readonly bool _desc;

		public OrderByDbExpressionVisitor(Model model, DynamicParameters parameters, bool desc = false)
			: base(model, parameters)
		{
			_desc = desc;
		}

		protected override Expression VisitNew(NewExpression node)
		{
			foreach (var item in node.Arguments)
			{
				Visit(item);
				if (item != node.Arguments.Last())
				{
					SetSql($" {OrderType}");
					SetSql(", ");
				}
			}
			return node;
		}

		public override string Build(Expression expression)
		{
			var order = base.Build(expression);
			return string.Format("{0} {1}", order, OrderType);
		}

		private string OrderType
		{
			get 
			{
				if (_desc)
				{
					return "DESC";
				}
				return "ASC";
			}
		}
	}
}
