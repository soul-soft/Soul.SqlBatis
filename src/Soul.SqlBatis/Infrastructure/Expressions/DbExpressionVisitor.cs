using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Soul.SqlBatis.Infrastructure
{
    public class DbExpressionVisitor : ExpressionVisitor
    {
        protected IModel Model { get; }

        protected DynamicParameters Parameters { get; } = new DynamicParameters();

        private readonly StringBuilder _buffer = new StringBuilder();

        public DbExpressionVisitor(IModel model, DynamicParameters parameters)
        {
            Model = model;
            Parameters = parameters;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is SqlToken raw)
            {
                SetSql(raw.Raw);
            }
            else
            {
                SetParameter(node);
            }
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not)
            {
                SetNot();
                SetBlank();
                SetLeftInclude();
                Visit(node.Operand);
                SetRightInclude();
            }
            else
            {
                Visit(node.Operand);
            }
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            SetLeftInclude();
            if (IsNullExpression(node))
            {
                var memberExpression = IsParameterMemberExpression(node.Left) ? node.Left : node.Right;
                Visit(memberExpression);
                SetBlank();
                SetIsNULL();
            }
            else if (IsNotNullExpression(node))
            {
                var memberExpression = IsParameterMemberExpression(node.Left) ? node.Left : node.Right;
                Visit(memberExpression);
                SetBlank();
                SetIsNotNULL();
            }
            else
            {
                Visit(node.Left);
                SetBlank();
                SetBinaryType(node.NodeType);
                SetBlank();
                Visit(node.Right);
            }
            SetRightInclude();
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (IsParameterMemberExpression(node))
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
            if (IsDbDbOperation(node))
            {
                var expression = new OperationDbExpressionVisitor(Model, Parameters).Build(node);
                SetSql(expression);
            }
            else if (IsDbFunction(node))
            {
                var expression = new FunctionDbExpressionVisitor(Model, Parameters).Build(node);
                SetSql(expression);
            }
            else
            {
                SetParameter(node);
            }
            return node;
        }

        protected bool IsNullExpression(BinaryExpression expression)
        {
            return (expression.NodeType == ExpressionType.Equal) && ((expression.Left is ConstantExpression leftExpression && leftExpression.Value == null) || (expression.Right is ConstantExpression rightExpression && rightExpression.Value == null));
        }

        protected bool IsNotNullExpression(BinaryExpression expression)
        {
            return (expression.NodeType == ExpressionType.NotEqual) && ((expression.Left is ConstantExpression leftExpression && leftExpression.Value == null) || (expression.Right is ConstantExpression rightExpression && rightExpression.Value == null));
        }

        protected bool IsDbFunction(MethodCallExpression expression)
        {
            if (expression.Method.CustomAttributes.Any(a => a.AttributeType == typeof(DbFunctionAttribute)))
            {
                return true;
            }
            return false;
        }

        protected bool IsDbDbOperation(MethodCallExpression expression)
        {
            if (expression.Method.DeclaringType == typeof(DbOperations))
            {
                return true;
            }
            return false;
        }

        protected bool IsParameterMemberExpression(Expression expression)
        {
            if (!(expression is MemberExpression))
            {
                return false;
            }
            var member = expression as MemberExpression;
            if (member.Expression != null && member.Expression.NodeType == ExpressionType.Parameter)
            {
                return true;
            }
            return false;
        }

        protected void SetLeftInclude()
        {
            _buffer.Append('(');
        }

        protected void SetRightInclude()
        {
            _buffer.Append(')');
        }

        protected void SetSql(string sql)
        {
            _buffer.Append(sql);
        }

        protected void SetIn()
        {
            _buffer.Append("IN");
        }

        protected void SetNot()
        {
            _buffer.Append("NOT");
        }

        protected void SetLike()
        {
            _buffer.Append("LIKE");
        }

        protected void SetBetween()
        {
            _buffer.Append("BETWEEN");
        }

        protected void SetIsNULL()
        {
            _buffer.Append("IS NULL");
        }

        protected void SetIsNotNULL()
        {
            _buffer.Append("IS NOT NULL");
        }

        protected void SetBlank()
        {
            _buffer.Append(' ');
        }

        protected void SetColumn(MemberInfo member)
        {
            var column = GetColumn(member);
            _buffer.Append(column);
        }

        protected string GetColumn(MemberInfo member)
        {
            var entity = Model.GetEntityType(member.DeclaringType);
            var property = entity.GetProperty(member)
                ?? throw new ModelException(string.Format("'{0}.{1}' is not mapped", member.DeclaringType.Name, member.Name)); ;

            return property.ColumnName;
        }

        protected virtual void SetParameter(Expression expression)
        {
            var value = GetParameter(expression);
            var name = Parameters.AddAnonymous(value);
            _buffer.Append($"@{name}");
        }

        public void SetBinaryType(ExpressionType expressionType)
        {
            _buffer.Append(GetBinaryType(expressionType));
        }

        protected object GetParameter(Expression expression)
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

        protected string GetBinaryType(ExpressionType expressionType)
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

        public virtual string Build(Expression expression)
        {
            if (expression is ConstantExpression constantExpression && constantExpression.Value is SqlToken sql)
            {
                _buffer.Append(sql.Raw);
            }
            else
            {
                Visit(expression);
            }
            return _buffer.ToString();
        }

        protected string Build(string text)
        {
            _buffer.Append(text);
            return _buffer.ToString();
        }
    }
}
