using Soul.SqlBatis.Infrastructure;
using Soul.SqlBatis.Metadata;

namespace Soul.SqlBatis.Expressions
{
    public class SqlExpressionContext
    {
        public string Alias { get; }
        public IModel Model { get; }
        public SqlSettings SqlSettings { get; }
        public DynamicParameters Parameters { get; }

        public SqlExpressionContext(string alias, IModel model, DynamicParameters parameters, SqlSettings sqlSettings)
        {
            Alias = alias;
            Model = model;
            Parameters = parameters;
            SqlSettings = sqlSettings;
        }
    }
}
