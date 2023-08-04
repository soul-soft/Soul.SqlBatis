using System.Collections.Generic;

namespace Soul.SqlBatis.Infrastructure
{
    public class DbExpressionBuilder
    {
        private readonly EntityModel _model;
       
        private readonly Dictionary<string, object> _parameters;

        public DbExpressionBuilder(EntityModel model, Dictionary<string, object> parametsers)
        {
            _model = model;
            _parameters = parametsers;
        }

        public string Build(DbExpression expression)
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
            if (expression.ExpressionType == DbExpressionType.Select)
            {
                var visitor = new SelectDbExpressionVisitor(_model, _parameters);
                return visitor.Build(expression.Expression);
            }
            return string.Empty;
        }
    }
}
