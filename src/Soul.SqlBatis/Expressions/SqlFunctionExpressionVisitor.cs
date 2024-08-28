using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Soul.SqlBatis.Expressions
{
    internal class SqlFunctionExpressionVisitor : SqlExpressionParser
    {
        protected SqlFunctionExpressionVisitor(SqlExpressionContext context)
            : base(context)
        {

        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var value = node.Value;
            if (value == null)
            {
                SqlBuilder.Append("NULL");
            }
            else if (node.Type == typeof(string))
            {
                SqlBuilder.Append($"'{value}'");
            }
            else
            {
                SqlBuilder.Append($"{value}");
            }
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var functionAttribute = node.Method.GetCustomAttribute<DbFunctionAttribute>() ?? new DbFunctionAttribute();
            var functionName = !string.IsNullOrEmpty(functionAttribute.Name) ? functionAttribute.Name : node.Method.Name;
            var parameters = node.Method.GetParameters();
            var arguments = new Dictionary<string, string>();
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                var name = parameters[i].Name;
                var value = Parse(Context, node.Arguments[i]);
                arguments.Add(name, value);
            }
            if (!string.IsNullOrEmpty(functionAttribute.Format))
            {
                var sb = new StringBuilder(functionAttribute.Format);

                foreach (var item in arguments)
                {
                    // 仅在格式字符串中包含键时才进行替换
                    if (functionAttribute.Format.Contains($"{{{item.Key}}}"))
                    {
                        sb.Replace($"{{{item.Key}}}", item.Value);
                    }
                }

                SqlBuilder.Append($"{functionName}({sb})");
            }
            else
            {
                SqlBuilder.Append($"{functionName}({string.Join(", ", arguments.Values)})");
            }
            return node;
        }

        public new static string Parse(SqlExpressionContext context, Expression expression)
        {
            var parser = new SqlFunctionExpressionVisitor(context);
            parser.Visit(expression);
            return parser.SqlBuilder.ToString(); // 返回构建的 SqlBuilder 实例
        }
    }
}
