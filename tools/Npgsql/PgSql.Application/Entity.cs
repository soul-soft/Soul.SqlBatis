namespace Soul.SqlBatis.Entities
{
    public abstract class Entity
    {
        public virtual int? Id { get; set; }
    }

    public abstract class Entity<T>
    {
        public virtual T? Id { get; set; }
    }
}
