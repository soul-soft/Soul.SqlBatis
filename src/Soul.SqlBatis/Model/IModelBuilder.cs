using Soul.SqlBatis.Linq;

namespace Soul.SqlBatis.Model
{
    public interface IModelBuilder
    {
        IEntityBuilder<T> Entity<T>();
        IModel Build();
    }
}
