using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Expressions
{
    public abstract class DbExpressionVisitor : ExpressionVisitor
    {
        protected  Model Model { get; }

        protected Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        private readonly StringBuilder _buffer = new StringBuilder();

        public DbExpressionVisitor(Model model, Dictionary<string, object> parameters)
        {
            Model = model;
            Parameters = parameters;
        }

        protected void Sql(string sql)
        {
            _buffer.Append(sql);
        }

        protected void BlankSpace()
        {
            _buffer.Append(' ');
        }

        protected void SetBinaryType(ExpressionType expressionType)
        {
            var value = DbExpressionUtility.GetDbExpressionType(expressionType);
            _buffer.Append(value);
        }

        protected void SetColumn(MemberInfo member)
        {
            var column = GetColumn(member);
            _buffer.Append(column);
        }
      
        protected string GetColumn(MemberInfo member)
        {
            var entity = Model.GetEntity(member.DeclaringType);
            return entity.GetProperty(member).ColumnName;
        }

        protected void SetParameter(object value)
        {
            var name = $"@P_{Parameters.Count}";
            _buffer.Append(name);
            Parameters.Add(name, value);
        }

        public virtual string Build(Expression expression)
        {
            if (expression is ConstantExpression constantExpression && constantExpression.Value is DbSyntax syntax)
            {
                _buffer.Append(syntax.Raw);
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
