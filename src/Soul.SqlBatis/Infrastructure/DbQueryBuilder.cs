using System.Collections.Generic;
using System.Linq;
using Soul.SqlBatis.Expressions;
using Soul.SqlBatis.Linq;

namespace Soul.SqlBatis
{
    public abstract class DbQueryBuilder
    {
        public List<DbExpression> Expressions { get; } = new List<DbExpression>();

        private Dictionary<string, object> _parameters = new Dictionary<string, object>();

        public DbContext DbContext { get; }

        public DbQueryBuilder(DbContext context)
        {
            DbContext = context;
        }

        public DbQueryBuilder(DbContext context, List<DbExpression> expressions)
        {
            DbContext = context;
            Expressions = expressions;
        }

        protected void AddParameter(object param)
        {
            if (param == null)
            {
                return;
            }
            if (param is Dictionary<string, object> values)
            {
                foreach (var item in values)
                {
                    _parameters.Add(item.Key, item.Value);
                }
            }
            var properties = param.GetType().GetProperties();
            foreach (var property in properties)
            {
                var name = property.Name;
                var value = property.GetValue(param);
                _parameters.Add(name, value);
            }
        }

        protected void AddExpression(DbExpression expression)
        {
            Expressions.Add(expression);
        }

        public DbCommand Build()
        {
            var engine = new DbExpressionEngine(DbContext.Model, _parameters);

            var list = Expressions.Select(s => new
            {
                Type = s.ExpressionType,
                Sql = engine.Build(s)
            }).ToList();


            return new DbCommand();
        }
    }
}
