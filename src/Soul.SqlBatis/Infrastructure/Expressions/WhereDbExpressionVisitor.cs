﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Expressions
{
    internal class WhereDbExpressionVisitor : DbExpressionVisitor
    {
        public WhereDbExpressionVisitor(Model model, Dictionary<string, object> parameters)
            : base(model, parameters)
        {

        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is DbSyntax syntax)
            {
                Sql(syntax.Raw);
            }
            else
            {
                SetParameter(node.Value);
            }
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Convert)
            {
                Visit(node.Operand);
            }
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);
            BlankSpace();
            SetBinaryType(node.NodeType);
            BlankSpace();
            Visit(node.Right);
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is ParameterExpression)
            {
                SetColumn(node.Member);
            }
            else
            {
                var value = DbExpressionUtility.GetDbExpressionValue(node);
                SetParameter(value);
            }
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var function = new FunctionDbExpression(Model, Parameters).Build(node);
            Sql(function);
            return node;
        }
    }
}