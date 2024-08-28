namespace Soul.SqlBatis.Expressions
{
    public class SqlExpressionContext
    {
        public string Alias { get; }
        public IModel Model { get; }
        public string EmptyQuerySql { get; }
        public DynamicParameters Parameters { get; }

        public SqlExpressionContext(string alias, IModel model, DynamicParameters parameters,string emptyQuerySql)
        {
            Alias = alias;
            Model = model;
            Parameters = parameters;
            EmptyQuerySql = emptyQuerySql;
        }
    }
}
