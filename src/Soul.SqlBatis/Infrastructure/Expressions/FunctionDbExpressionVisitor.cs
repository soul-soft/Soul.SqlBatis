using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    internal class FunctionDbExpressionVisitor : DbExpressionVisitor
    {
        public FunctionDbExpressionVisitor(IModel model, DynamicParameters parameters)
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
            else if(expression.Type == typeof(string))
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
            SetSql(functionName);
            SetLeftInclude();
            foreach (var item in node.Arguments)
            {
                if (item.Type == typeof(SqlToken))
                {
                    var value = GetParameter(item);
                    SetSql(value.ToString());
                }
                else
                {
                    Visit(item);
                }
                if (item != node.Arguments.Last())
                {
                    SetSql(", ");
                }
            }
            SetRightInclude();
            return node;
        }
    }
}
