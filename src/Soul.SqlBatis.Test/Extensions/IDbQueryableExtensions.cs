namespace Soul.SqlBatis.Test.Extensions
{
    public class Orders : Dictionary<string, int>
    {

    }


    internal static class IDbQueryableExtensions
    {
        public static IDbQueryable<T> OrderBy<T>(this IDbQueryable<T> queryable, Orders orders)
        {
            var query = queryable as DbQueryable<T>
                ?? throw new InvalidCastException();
            var entityType = query.EntityType;
            var properties = entityType.GetProperties();
            foreach (var item in orders)
            {
                var property = properties.Where(a => a.Property.Name.Equals(item.Key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (property == null)
                {
                    continue;
                }
                if (item.Value == 0)
                {
                    queryable.OrderBy(property.ColumnName);
                }
                else
                {
                    queryable.OrderBy($"{property.ColumnName} DESC");
                }
            }
            return queryable;
        }
    }
}
