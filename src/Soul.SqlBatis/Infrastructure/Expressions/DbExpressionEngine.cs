using System;
using System.Collections.Generic;
using Soul.SqlBatis.Expressions;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Linq
{
    public class DbExpressionBuilder
    {
        private readonly Model _model;
       
        private readonly Dictionary<string, object> _parameters;

        public DbExpressionBuilder(Model model, Dictionary<string, object> parametsers)
        {
            _model = model;
            _parameters = parametsers;
        }

        public string Build(DbExpression expression)
        {
			if (expression.ExpressionType == DbExpressionType.From)
			{
				var visitor = new WhereDbExpressionVisitor(_model, _parameters);
				return visitor.Build(expression.Expression);
			}
			if (expression.ExpressionType == DbExpressionType.Where)
            {
                var visitor = new WhereDbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            if (expression.ExpressionType == DbExpressionType.Select)
            {
                var visitor = new SelectDbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            return string.Empty;
        }
    }
}
