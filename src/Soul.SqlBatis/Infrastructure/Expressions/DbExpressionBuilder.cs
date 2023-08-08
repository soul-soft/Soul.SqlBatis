using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    public class DbExpressionBuilder
    {
        private readonly Model _model;

        private readonly Dictionary<string, object> _parameters;

        private readonly IEnumerable<DbExpression> _expressions;

        public DbExpressionBuilder(Model model, Dictionary<string, object> parametsers, IEnumerable<DbExpression> expressions)
        {
            _model = model;
            _parameters = parametsers;
            _expressions = expressions;
        }

        private string Build(DbExpression expression)
        {
            if (expression.ExpressionType == DbExpressionType.From)
            {
                var visitor = new DbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionType.Where)
            {
                var visitor = new DbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionType.OrderBy)
            {
                var visitor = new OrderByDbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionType.Skip)
            {
                var visitor = new DbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionType.Take)
            {
                var visitor = new DbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionType.GroupBy)
            {
                var visitor = new DbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionType.Having)
            {
                var visitor = new DbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionType.OrderByDescending)
            {
                var visitor = new OrderByDbExpressionVisitor(_model, _parameters, true);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionType.Select)
            {
                var visitor = new SelectDbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            throw new NotImplementedException();
        }

        public Dictionary<DbExpressionType, IEnumerable<string>> Build()
        {
            var result = new Dictionary<DbExpressionType, IEnumerable<string>>();
            var values = _expressions.Select(s =>
            {
                var key = s.ExpressionType;
                if (key == DbExpressionType.OrderByDescending)
                {
                    key = DbExpressionType.OrderBy;
                }
                var value = Build(s);
                return new
                {
                    ExpressionType = key,
                    Expression = value
                };
            }).GroupBy(s => s.ExpressionType);
            foreach (var item in values)
            {
                result.Add(item.Key, item.Select(s => s.Expression));
            }
            return result;
        }
    }
}
