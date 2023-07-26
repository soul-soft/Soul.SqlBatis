using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Expressions
{
    public abstract class SqlExpressionVisitor : ExpressionVisitor
    {
        private readonly Model _model;

        private readonly StringBuilder _buffer = new StringBuilder();

        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

        protected SqlExpressionVisitor(Model model)
        {
            _model = model;
        }

        protected SqlExpressionVisitor(Dictionary<string, object> parameters)
        {
            _parameters = parameters;
        }

        public abstract string Build(Expression expression, Dictionary<string, object> parameters);

        protected void SetBinaryType(ExpressionType expressionType)
        {
            var value = SqlExpressionUtility.GetSqlExpressionType(expressionType);
            _buffer.Append(value);
        }

        protected void SetColumn(MemberExpression expression)
        {
            var entity = _model.GetEntity(expression.Type);
            var column = entity.GetProperty(expression.Member).ColumnName;
            _buffer.Append(column);
        }

        protected void SetParameter(MemberExpression expression, object value)
        {
            var name = $"@{expression.Member.Name}_{_parameters.Count}";
            _buffer.Append(name);
            _parameters.Add(name, value);
        }
    }
}
