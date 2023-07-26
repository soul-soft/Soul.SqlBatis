using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Soul.SqlBatis.Expressions
{
    internal static class DbExpressionUtility
    {
        public static object GetDbExpressionValue(Expression expression)
        {
            object value = null;
            if (expression is ConstantExpression constant)
                value = constant.Value;
            else if (expression is MemberExpression)
            {
                var expressions = new Stack<MemberExpression>();
                var temp = expression;
                while (temp is MemberExpression memberExpression)
                {
                    expressions.Push(memberExpression);
                    temp = memberExpression.Expression;
                }
                foreach (var item in expressions)
                {
                    if (item.Expression is ConstantExpression constantExpression)
                        value = constantExpression.Value;
                    if (item.Member is PropertyInfo property)
                        value = property.GetValue(value);
                    else if (item.Member is FieldInfo field)
                        value = field.GetValue(value);
                }
            }
            else
            {
                value = Expression.Lambda(expression).Compile().DynamicInvoke();
            }
           
            return value;
        }

        public static string GetDbExpressionType(ExpressionType expressionType)
        {
            string result = string.Empty;
            switch (expressionType)
            {
                case ExpressionType.Add:
                    result = "+";
                    break;
                case ExpressionType.AddAssign:
                    break;
                case ExpressionType.AddAssignChecked:
                    break;
                case ExpressionType.AddChecked:
                    result = "+";
                    break;
                case ExpressionType.And:
                    result = "&";
                    break;
                case ExpressionType.AndAlso:
                    result = "AND";
                    break;
                case ExpressionType.AndAssign:
                    break;
                case ExpressionType.ArrayIndex:
                    break;
                case ExpressionType.ArrayLength:
                    break;
                case ExpressionType.Assign:
                    break;
                case ExpressionType.Block:
                    break;
                case ExpressionType.Call:
                    break;
                case ExpressionType.Coalesce:
                    break;
                case ExpressionType.Conditional:
                    break;
                case ExpressionType.Constant:
                    break;
                case ExpressionType.Convert:
                    break;
                case ExpressionType.ConvertChecked:
                    break;
                case ExpressionType.DebugInfo:
                    break;
                case ExpressionType.Decrement:
                    break;
                case ExpressionType.Default:
                    break;
                case ExpressionType.Divide:
                    result = "/";
                    break;
                case ExpressionType.DivideAssign:
                    break;
                case ExpressionType.Dynamic:
                    break;
                case ExpressionType.Equal:
                    result = "=";
                    break;
                case ExpressionType.ExclusiveOr:
                    result = "^";
                    break;
                case ExpressionType.ExclusiveOrAssign:
                    break;
                case ExpressionType.Extension:
                    break;
                case ExpressionType.Goto:
                    break;
                case ExpressionType.GreaterThan:
                    result = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    result = ">=";
                    break;
                case ExpressionType.Increment:
                    break;
                case ExpressionType.Index:
                    break;
                case ExpressionType.Invoke:
                    break;
                case ExpressionType.IsFalse:
                    break;
                case ExpressionType.IsTrue:
                    break;
                case ExpressionType.Label:
                    break;
                case ExpressionType.Lambda:
                    break;
                case ExpressionType.LeftShift:
                    result = "<<";
                    break;
                case ExpressionType.LeftShiftAssign:
                    break;
                case ExpressionType.LessThan:
                    result = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    result = "<=";
                    break;
                case ExpressionType.ListInit:
                    break;
                case ExpressionType.Loop:
                    break;
                case ExpressionType.MemberAccess:
                    break;
                case ExpressionType.MemberInit:
                    break;
                case ExpressionType.Modulo:
                    result = "%";
                    break;
                case ExpressionType.ModuloAssign:
                    break;
                case ExpressionType.Multiply:
                    result = "*";
                    break;
                case ExpressionType.MultiplyAssign:
                    break;
                case ExpressionType.MultiplyAssignChecked:
                    break;
                case ExpressionType.MultiplyChecked:
                    result = "*";
                    break;
                case ExpressionType.Negate:
                    break;
                case ExpressionType.NegateChecked:
                    break;
                case ExpressionType.New:
                    break;
                case ExpressionType.NewArrayBounds:
                    break;
                case ExpressionType.NewArrayInit:
                    break;
                case ExpressionType.Not:
                    result = "NOT";
                    break;
                case ExpressionType.NotEqual:
                    result = "<>";
                    break;
                case ExpressionType.OnesComplement:
                    break;
                case ExpressionType.Or:
                    result = "|";
                    break;
                case ExpressionType.OrAssign:
                    break;
                case ExpressionType.OrElse:
                    result = "OR";
                    break;
                case ExpressionType.Parameter:
                    break;
                case ExpressionType.PostDecrementAssign:
                    break;
                case ExpressionType.PostIncrementAssign:
                    break;
                case ExpressionType.Power:
                    result = "^";
                    break;
                case ExpressionType.PowerAssign:
                    break;
                case ExpressionType.PreDecrementAssign:
                    break;
                case ExpressionType.PreIncrementAssign:
                    break;
                case ExpressionType.Quote:
                    break;
                case ExpressionType.RightShift:
                    result = ">>";
                    break;
                case ExpressionType.RightShiftAssign:
                    break;
                case ExpressionType.RuntimeVariables:
                    break;
                case ExpressionType.Subtract:
                    result = "-";
                    break;
                case ExpressionType.SubtractAssign:
                    break;
                case ExpressionType.SubtractAssignChecked:
                    break;
                case ExpressionType.SubtractChecked:
                    result = "-";
                    break;
                case ExpressionType.Switch:
                    break;
                case ExpressionType.Throw:
                    break;
                case ExpressionType.Try:
                    break;
                case ExpressionType.TypeAs:
                    break;
                case ExpressionType.TypeEqual:
                    break;
                case ExpressionType.TypeIs:
                    break;
                case ExpressionType.UnaryPlus:
                    break;
                case ExpressionType.Unbox:
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(result))
            {
                throw new InvalidOperationException();
            }
            return result;
        }
    }
}
