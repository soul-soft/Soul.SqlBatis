﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Expressions
{
    internal class FunctionDbExpressionVisitor : DbExpressionVisitor
    {
        public FunctionDbExpressionVisitor(Model model, Dictionary<string, object> parameters)
            : base(model, parameters)
        {

        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is DbSql syntax)
            {
                Sql(syntax.Raw);
            }
            else
            {
                Sql(node.Value.ToString());
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
            SetBlankSpace();
            SetBinaryType(node.NodeType);
            SetBlankSpace();
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
                SetParameter(node);
            }
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.CustomAttributes.Any(a => a.AttributeType == typeof(DbFunctionAttribute))
                || node.Method.DeclaringType.CustomAttributes.Any(a => a.AttributeType == typeof(DbFunctionAttribute)))
            {
                Sql(node.Method.Name);
                Sql("(");
                foreach (var item in node.Arguments)
                {
                    Visit(item);
                    if (item != node.Arguments.Last())
                    {
                        Sql(", ");
                    }
                }
                Sql(")");
            }
            else if(node.Method.DeclaringType == typeof(DbFunc) && node.Method.Name == nameof(DbFunc.Raw))
            {
                Visit(node.Arguments[0]);
            }

            return node;
        }
    }
}
