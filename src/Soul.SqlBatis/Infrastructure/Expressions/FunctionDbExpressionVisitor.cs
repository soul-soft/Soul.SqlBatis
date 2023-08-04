﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    internal class FunctionDbExpressionVisitor : DbExpressionVisitor
    {
        public FunctionDbExpressionVisitor(Model model, Dictionary<string, object> parameters)
            : base(model, parameters)
        {

        }

        protected override void SetParameter(Expression expression)
        {
            var value = GetParameter(expression);
            SetSql(value.ToString());
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (IsDbOperation(node))
            {
                var sql = BuildDbOperation(node);
                SetSql(sql);
            }
            else if (IsDbFunction(node))
            {
                var functionAttribute = node.Method.GetCustomAttribute<DbFunctionAttribute>();
                var functionName = !string.IsNullOrEmpty(functionAttribute.Name) ? functionAttribute.Name : node.Method.Name;
                SetSql(functionName);
                SetSql("(");
                foreach (var item in node.Arguments)
                {
                    Visit(item);
                    if (item != node.Arguments.Last())
                    {
                        SetSql(", ");
                    }
                }
                SetSql(")");
            }
            else
            {
                SetParameter(node);
            }
            return node;
        }

        private bool IsDbFunction(MethodCallExpression expression)
        {
            return expression.Method.CustomAttributes.Any(a => a.AttributeType == typeof(DbFunctionAttribute));
        }

        private bool IsDbOperation(MethodCallExpression expression)
        {
            return expression.Method.DeclaringType == typeof(DbOperations);
        }
    }
}
