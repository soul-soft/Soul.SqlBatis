using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Soul.SqlBatis.Infrastructure
{
    internal class DbFunctionExpressionVisitor : DbExpressionVisitor
    {
        public DbFunctionExpressionVisitor(IModel model, DynamicParameters parameters)
            : base(model, parameters)
        {

        }

        protected override void SetParameter(Expression expression)
        {
            var value = GetParameter(expression);
            if (value == null)
            {
                SetSql("NULL");
            }
            else if (expression.Type == typeof(string))
            {
                SetSql($"'{value}'");
            }
            else
            {
                SetSql(value.ToString());
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var functionAttribute = node.Method.GetCustomAttribute<DbFunctionAttribute>();
            var functionName = !string.IsNullOrEmpty(functionAttribute.Name) ? functionAttribute.Name : node.Method.Name;
            var parameters = node.Method.GetParameters();
            var arguments = new Dictionary<string, string>();
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                var name = parameters[i].Name;
                var value = GetArgumentSql(node.Arguments[i]);
                arguments.Add(name, value);
            }
            if (functionAttribute.Format != null)
            {
                var args = Regex.Replace(functionAttribute.Format, @"\{(?<name>[a-zA-Z_]{0}[a-zA-Z_][a-zA-Z_0-9]+?)\}", m =>
                {
                    var name = m.Groups["name"].Value;
                    if (!arguments.ContainsKey(name))
                    {
                        throw new DbExpressionException(string.Format("Cannot find parameter named '{0}' in the '{1}({2})' function", name, functionName, string.Join(" ,", parameters.Select(s => s.Name))));
                    }
                    return arguments[name];
                });
                SetSql($"{functionName}({args})");
            }
            else
            {
                SetSql($"{functionName}({string.Join(" ,", arguments.Values)})");
            }
            return node;
        }

        private string GetArgumentSql(Expression expression)
        {
            var visitor = new DbExpressionVisitor(Model, Parameters);
            return visitor.Build(expression);
        }
    }
}
