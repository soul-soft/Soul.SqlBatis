namespace Soul.SqlBatis.Test
{
    internal static class QueryableExtensions
    {
        public static IDbQueryable<T> OrderBy<T>(this IDbQueryable<T> query, QueryModel model)
        {
            var entityType = query.Model.GetEntityType(typeof(T));
            foreach (var item in model.Orders)
            {
                var property = entityType.Properties
                    .Where(a => a.CSharpName.ToUpper() == item.Key.ToUpper())
                    .FirstOrDefault();
                if (property == null)
                {
                    continue;
                }
                if (model.Orders.IsAsc(item.Key))
                {
                    query.OrderBy($"{property.ColumnName}");
                }
                else
                {
                    query.OrderBy($"{property.ColumnName} DESC");
                }
            }
            return query;
        }
    }
}
