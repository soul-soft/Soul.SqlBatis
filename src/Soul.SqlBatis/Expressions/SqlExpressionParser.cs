using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Soul.SqlBatis.Expressions
{
    public class SqlExpressionParser : ExpressionVisitor
    {
        protected SqlExpressionContext Context { get; private set; }

        protected StringBuilder SqlBuilder { get; private set; } = new StringBuilder();


        private readonly Dictionary<ExpressionType, string> _sqlOperators = new Dictionary<ExpressionType, string>
        {
            // 比较运算符
            { ExpressionType.Equal, "=" },
            { ExpressionType.NotEqual, "!=" },
            { ExpressionType.GreaterThan, ">" },
            { ExpressionType.GreaterThanOrEqual, ">=" },
            { ExpressionType.LessThan, "<" },
            { ExpressionType.LessThanOrEqual, "<=" },

            // 逻辑运算符
            { ExpressionType.AndAlso, "AND" },
            { ExpressionType.OrElse, "OR" },
            { ExpressionType.Not, "NOT" },

            // 算术运算符
            { ExpressionType.Add, "+" },
            { ExpressionType.Subtract, "-" },
            { ExpressionType.Multiply, "*" },
            { ExpressionType.Divide, "/" },
            { ExpressionType.Modulo, "%" },

            // 位运算符
            { ExpressionType.And, "&" },
            { ExpressionType.Or, "|" },
            { ExpressionType.ExclusiveOr, "^" },
            { ExpressionType.OnesComplement, "~" },
            { ExpressionType.LeftShift, "<<" },
            { ExpressionType.RightShift, ">>" }
        };


        protected SqlExpressionParser(SqlExpressionContext context)
        {
            Context = context;
        }

        public static string Parse(SqlExpressionContext context, Expression expression)
        {
            var parser = new SqlExpressionParser(context);
            parser.Visit(expression);
            return parser.SqlBuilder.ToString(); // 返回构建的 SqlBuilder 实例
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            SqlBuilder.Append("(");
            Visit(node.Left);
            if (node.NodeType == ExpressionType.Equal && (IsNullConstant(node.Right) || IsNullConstant(node.Left)))
            {
                SqlBuilder.Append($" IS ");
            }
            else if (node.NodeType == ExpressionType.NotEqual && (IsNullConstant(node.Right) || IsNullConstant( node.Left)))
            {
                SqlBuilder.Append($" IS NOT ");
            }
            else
            {
                SqlBuilder.Append($" {GetSqlOps(node.NodeType)} ");
            }
            Visit(node.Right);
            SqlBuilder.Append(")");
            return node;
        }

        private bool IsNullConstant(Expression expression)
        {
            if (expression is ConstantExpression constantExpression)
            {
                return constantExpression.Value == null;
            }
            else
            {
                return false;
            }
        }


        protected override Expression VisitMember(MemberExpression node)
        {
            if (IsParameterExpression(node))
            {
                var entityType = Context.Model.FindEntityType(node.Expression.Type);
                var property = entityType.GetProperty(node.Member.Name);
                var columnName = property.ColumnName;
                if (!string.IsNullOrEmpty(Context.Alias))
                {
                    columnName = $"{Context.Alias}.{columnName}";
                }
                SqlBuilder.Append(columnName);
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
                SqlBuilder.Append(GetSqlOps(ExpressionType.Not));
                SqlBuilder.Append('(');
                Visit(node.Operand);
                SqlBuilder.Append(')');
            }
            else
            {
                Visit(node.Operand);
            }
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value != null)
            {
                if (node.Type == typeof(string))
                {
                    SetParameter(node);
                }
                else
                {
                    SqlBuilder.Append($"{node.Value}");
                }
            }
            else
            {
                SqlBuilder.Append("NULL");
            }
            return node;
        }

        private void SetParameter(Expression expression)
        {
            var value = ParseValue(expression);
            var paramName = $"P{Context.Parameters.Count}";
            Context.Parameters.Add(paramName, value);
            SqlBuilder.Append($"@{paramName}");
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            //in
            if (node.Method.DeclaringType == typeof(Enumerable)
                && node.Arguments[0].Type != typeof(string) 
                && node.Method.Name == nameof(Enumerable.Contains) 
                && IsParameterExpression(node.Arguments[1]))
            {
                var value = ParseValue(node.Arguments[0]) as IEnumerable;
                var values = new List<object>();
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        values.Add(item);
                    }
                }
                SetInQuery(node.Arguments[1], values);
            } 
            //in
            else if (typeof(ICollection).IsAssignableFrom(node.Method.DeclaringType)
                && node.Object.Type != typeof(string)
                && node.Method.Name == nameof(Enumerable.Contains)
                && IsParameterExpression(node.Arguments[0]))
            {
                var value = ParseValue(node.Object) as IEnumerable;
                var values = new List<object>();
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        values.Add(item);
                    }
                }
                SetInQuery(node.Arguments[0], values);
            }
            //contains
            else if (node.Method.DeclaringType == typeof(string) && node.Method.Name == nameof(string.Contains))
            {
                Visit(node.Object);
                SqlBuilder.Append(" LIKE ");
                var value = ParseValue(node.Arguments[0]);
                if (value != null)
                    SetParameter(Expression.Constant($"%{value}%"));
                else
                    SetParameter(Expression.Constant(value));
            }
            else if (node.Method.DeclaringType == typeof(string) && node.Method.Name == nameof(string.StartsWith))
            {
                Visit(node.Object);
                SqlBuilder.Append(" LIKE ");
                var value = ParseValue(node.Arguments[0]);
                if (value != null)
                    SetParameter(Expression.Constant($"{value}%"));
                else
                    SetParameter(Expression.Constant(value));
            }
            else if (node.Method.DeclaringType == typeof(string) && node.Method.Name == nameof(string.EndsWith))
            {
                Visit(node.Object);
                SqlBuilder.Append(" LIKE ");
                var value = ParseValue(node.Arguments[0]);
                if (value != null)
                    SetParameter(Expression.Constant($"%{value}"));
                else
                    SetParameter(Expression.Constant(value));
            }
            else if (node.Method.DeclaringType == typeof(DbOps))
            {
                if (node.Method.Name == nameof(DbOps.Contains) && node.Arguments.Count == 2)
                {
                    Visit(node.Arguments[0]);
                    SqlBuilder.Append(" LIKE ");
                    var value = ParseValue(node.Arguments[1]);
                    if (value != null)
                        SetParameter(Expression.Constant($"%{value}%"));
                    else
                        SetParameter(Expression.Constant(value));
                }
                else if (node.Method.Name == nameof(DbOps.StartWith) && node.Arguments.Count == 2)
                {
                    Visit(node.Arguments[0]);
                    SqlBuilder.Append(" LIKE ");
                    var value = ParseValue(node.Arguments[1]);
                    if (value != null)
                        SetParameter(Expression.Constant($"{value}%"));
                    else
                        SetParameter(Expression.Constant(value));
                }
                else if (node.Method.Name == nameof(DbOps.EndWith) && node.Arguments.Count == 2)
                {
                    Visit(node.Arguments[0]);
                    SqlBuilder.Append(" LIKE ");
                    var value = ParseValue(node.Arguments[1]);
                    if (value != null)
                        SetParameter(Expression.Constant($"%{value}"));
                    else
                        SetParameter(Expression.Constant(value));
                }
                else if (node.Method.Name == nameof(DbOps.In) && node.Arguments.Count == 2)
                {
                    var value = ParseValue(node.Arguments[1]) as IEnumerable;
                    var values = new List<object>();
                    if (value != null)
                    {
                        foreach (var item in value)
                        {
                            values.Add(item);
                        }
                    }
                    SetInQuery(node.Arguments[0], values);
                }
                else if (node.Method.Name == nameof(DbOps.InSet) && node.Arguments.Count == 2)
                {
                    var type = Nullable.GetUnderlyingType(node.Arguments[0].Type) ?? node.Arguments[0].Type;
                    var value = ParseValue(node.Arguments[1]).ToString();
                    var values = value.ToString().Split(',')
                        .Select(s => type == typeof(string) ? s : Convert.ChangeType(s, type))
                        .ToList();
                    SetInQuery(node.Arguments[0], values);
                }
                else if (node.Method.Name == nameof(DbOps.InSub) && node.Arguments.Count == 2)
                {
                    Visit(node.Arguments[0]);
                    SqlBuilder.Append(" IN ");
                    var sub = ParseValue(node.Arguments[1]);
                    SqlBuilder.Append($"({sub})");
                }
            }
            else if (node.Method.DeclaringType.GetCustomAttribute<DbFunctionAttribute>() != null || node.Method.GetCustomAttribute<DbFunctionAttribute>() != null)
            {
                var sql = SqlFunctionExpressionVisitor.Parse(Context, node);
                SqlBuilder.Append(sql);
            }
            else
            {
                SetParameter(node);
            }

            return node;
        }

        private void SetInQuery(Expression member, List<object> values)
        {
            Visit(member);
            SqlBuilder.Append(" IN ");
            if (values.Count == 0)
            {
                SqlBuilder.Append($"({Context.EmptyQuerySql})");
            }
            else if (member.Type != typeof(string))
            {
                SqlBuilder.Append($"({string.Join(",", values)})");
            }
            else
            {
                SetParameter(Expression.Constant(values));
            }
        }

        private string GetSqlOps(ExpressionType type)
        {
            if (_sqlOperators.TryGetValue(type, out var sqlOperator))
            {
                return sqlOperator;
            }
            throw new NotSupportedException($"Operator {type} is not supported.");
        }

        private bool IsParameterExpression(Expression expression)
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

        private static object ParseValue(Expression expression)
        {
            var value = SqlValueExpressionParser.Parse(expression);

            return value;
        }
    }

}
