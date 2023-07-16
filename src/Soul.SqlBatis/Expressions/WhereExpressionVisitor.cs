using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Expressions
{
	internal class WhereExpressionVisitor : SqlExpressionVisitor
	{
		public WhereExpressionVisitor(Dictionary<string, object> parameters) 
			: base(parameters)
		{

		}

		public override string Build(Expression expression, Dictionary<string, object> parameters)
		{
			throw new NotImplementedException();
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			Visit(node.Left);
			Visit(node.Right);
			return base.VisitBinary(node);
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			return base.VisitMember(node);
		}
	}
}
