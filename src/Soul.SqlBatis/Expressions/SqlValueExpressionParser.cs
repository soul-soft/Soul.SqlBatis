using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Soul.SqlBatis.Expressions
{
    public class SqlValueExpressionParser
    {
        public static object Parse(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    return ((ConstantExpression)expression).Value;

                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;
                    var obj = memberExpression.Expression != null ? Parse(memberExpression.Expression) : null;
                    if (memberExpression.Member is PropertyInfo propertyInfo)
                    {
                        return propertyInfo.GetValue(obj);
                    }
                    else if (memberExpression.Member is FieldInfo fieldInfo)
                    {
                        return fieldInfo.GetValue(obj);
                    }
                    throw new InvalidOperationException("不支持的成员类型。");

                case ExpressionType.New:
                    var newExpression = (NewExpression)expression;
                    var args = new object[newExpression.Arguments.Count];
                    for (int i = 0; i < newExpression.Arguments.Count; i++)
                    {
                        args[i] = Parse(newExpression.Arguments[i]);
                    }
                    return newExpression.Constructor.Invoke(args);

                case ExpressionType.NewArrayInit:
                    var newArrayExpression = (NewArrayExpression)expression;
                    var array = Array.CreateInstance(newArrayExpression.Type.GetElementType(), newArrayExpression.Expressions.Count);
                    for (int i = 0; i < newArrayExpression.Expressions.Count; i++)
                    {
                        array.SetValue(Parse(newArrayExpression.Expressions[i]), i);
                    }
                    return array;

                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;
                    var instance = callExpression.Object != null ? Parse(callExpression.Object) : null;
                    var parameters = new object[callExpression.Arguments.Count];
                    for (int i = 0; i < callExpression.Arguments.Count; i++)
                    {
                        parameters[i] = Parse(callExpression.Arguments[i]);
                    }
                    return callExpression.Method.Invoke(instance, parameters);

                case ExpressionType.ListInit:
                    var listInitExpression = (ListInitExpression)expression;
                    var list = (IList)Parse(listInitExpression.NewExpression);
                    foreach (var initializer in listInitExpression.Initializers)
                    {
                        var argsList = new object[initializer.Arguments.Count];
                        for (int i = 0; i < initializer.Arguments.Count; i++)
                        {
                            argsList[i] = Parse(initializer.Arguments[i]);
                        }
                        initializer.AddMethod.Invoke(list, argsList);
                    }
                    return list;

                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;
                    var operand = Parse(unaryExpression.Operand);
                    return Convert.ChangeType(operand, Nullable.GetUnderlyingType(unaryExpression.Type) ?? unaryExpression.Type);

                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                    var binaryExpression = (BinaryExpression)expression;
                    var left = Parse(binaryExpression.Left);
                    var right = Parse(binaryExpression.Right);
                    return EvaluateBinaryExpression(binaryExpression.NodeType, left, right);

                default:
                    throw new NotSupportedException($"不支持的表达式类型: {expression.NodeType}");
            }
        }

        private static object EvaluateBinaryExpression(ExpressionType nodeType, object left, object right)
        {
            if (left == null || right == null)
            {
                throw new InvalidOperationException("操作数不能为 null。");
            }

            switch (nodeType)
            {
                case ExpressionType.Add:
                    if (left is int && right is int) return (int)left + (int)right;
                    if (left is double && right is double) return (double)left + (double)right;
                    // 添加更多类型支持
                    break;
                case ExpressionType.Subtract:
                    if (left is int && right is int) return (int)left - (int)right;
                    if (left is double && right is double) return (double)left - (double)right;
                    // 添加更多类型支持
                    break;
                case ExpressionType.Multiply:
                    if (left is int && right is int) return (int)left * (int)right;
                    if (left is double && right is double) return (double)left * (double)right;
                    // 添加更多类型支持
                    break;
                case ExpressionType.Divide:
                    if (left is int && right is int) return (int)left / (int)right;
                    if (left is double && right is double) return (double)left / (double)right;
                    // 添加更多类型支持
                    break;
                case ExpressionType.Modulo:
                    if (left is int && right is int) return (int)left % (int)right;
                    // 添加更多类型支持
                    break;
                default:
                    throw new NotSupportedException($"不支持的二元操作: {nodeType}");
            }

            throw new NotSupportedException("不支持的操作数类型。");
        }
    }
}