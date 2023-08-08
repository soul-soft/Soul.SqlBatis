using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Infrastructure
{
    internal class GroupByDbExpressionVisitor : DbExpressionVisitor
	{

		public GroupByDbExpressionVisitor(Model model, Dictionary<string, object> parameters)
			: base(model, parameters)
		{
		}

		protected override Expression VisitNew(NewExpression node)
		{
			foreach (var item in node.Arguments)
			{
				Visit(item);
				if (item!=node.Arguments.Last())
				{
					SetSql(", ");
				}
            }
			return node;
		}
	}
}
