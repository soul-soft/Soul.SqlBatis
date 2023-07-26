using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Expressions
{
    internal class WhereSqlExpressionVisitor : SqlExpressionVisitor
    {
        private readonly Model _model;

        public WhereSqlExpressionVisitor(Dictionary<string, object> parameters, Model model)
            : base(parameters)
        {
            _model = model;
        }

        public override string Build(Expression expression, Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);
            SetBinaryType(node.NodeType);
            Visit(node.Right);

            return base.VisitBinary(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (_model.IsEntity(node.Member.DeclaringType))
            {
                SetColumn(node);
            }
            else
            {
                SetParameter(node, null);
            }
            return base.VisitMember(node);
        }
    }
}
