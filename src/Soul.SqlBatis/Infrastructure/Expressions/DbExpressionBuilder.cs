using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Infrastructure
{
    internal class DbExpressionBuilder
    {
        private readonly IModel _model;

        private readonly DynamicParameters _parameters;

        private readonly IEnumerable<DbExpression> _expressions;

        public DbExpressionBuilder(IModel model, DynamicParameters parametsers, IEnumerable<DbExpression> expressions)
        {
            _model = model;
            _parameters = parametsers;
            _expressions = expressions;
        }
     
        public Dictionary<DbExpressionToken, IEnumerable<string>> Build()
        {
            var result = new Dictionary<DbExpressionToken, IEnumerable<string>>();
            var values = _expressions.Select(s =>
            {
                var key = s.ExpressionType;
                if (key == DbExpressionToken.OrderByDescending)
                {
                    key = DbExpressionToken.OrderBy;
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

        private string Build(DbExpression expression)
        {
            if (expression is DbSqlExpression sqlExpression)
            {
                return (sqlExpression.Expression as ConstantExpression).Value.ToString();
            }
            if (expression.ExpressionType == DbExpressionToken.From)
            {
                var visitor = new DbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionToken.Where)
            {
                var visitor = new DbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionToken.OrderBy)
            {
                var visitor = new DbOrderByExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionToken.Skip)
            {
                var visitor = new DbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionToken.Take)
            {
                var visitor = new DbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionToken.GroupBy)
            {
                var visitor = new DbGroupByExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionToken.Having)
            {
                var visitor = new DbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionToken.OrderByDescending)
            {
                var visitor = new DbOrderByExpressionVisitor(_model, _parameters, true);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionToken.Select)
            {
                var visitor = new DbSelectExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionToken.Set)
            {
                var setExpression = expression as DbSetExpression;
                var memberExpression = GetMemberExpression(setExpression.Expression);
                var columnName = _model.GetEntityType(memberExpression.Member.DeclaringType).GetProperty(memberExpression.Member).ColumnName;
                if (setExpression.Value is ConstantExpression constantExpression)
                {
                    var parameterName = _parameters.AddAnonymous(constantExpression.Value);
                    return $"{columnName} = @{parameterName}";
                }
                else
                {
                    var visitor = new DbExpressionVisitor(_model, _parameters);
                    var valueExpression = visitor.Build(setExpression.Value);
                    return $"{columnName} = {valueExpression}";
                }

            }
            throw new NotImplementedException();
        }

        private MemberExpression GetMemberExpression(Expression expression)
        {
            if (expression is LambdaExpression lambdaExpression)
            {
                return GetMemberExpression(lambdaExpression.Body);
            }
            if (expression is MemberExpression memberExpression)
            {
                return memberExpression;
            }
            throw new NotSupportedException();
        }
    }
}
