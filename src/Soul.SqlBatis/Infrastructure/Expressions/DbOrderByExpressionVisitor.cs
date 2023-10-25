using System.Linq;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Infrastructure
{
    internal class DbOrderByExpressionVisitor : DbExpressionVisitor
	{
		private readonly bool _desc;

		public DbOrderByExpressionVisitor(IModel model, DynamicParameters parameters, bool desc = false)
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
			return string.Join(" ", order, OrderType);
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
