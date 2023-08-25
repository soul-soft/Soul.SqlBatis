namespace Soul.SqlBatis.Test
{
    internal static class QueryableExtensions
    {
        public static (List<T>, int) ToPageList<T>(this IDbQueryable<T> query, int page, int size)
        {
            var offset = (page - 1) * size;
            var list = query.Take(size).Skip(offset).ToList();
            var count = query.Count();
            return (list, count);
        }

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
