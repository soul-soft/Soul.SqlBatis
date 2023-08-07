namespace Soul.SqlBatis.Infrastructure
{
    public class EntityEntry
    {
        public object Entity { get; }

        public EntityState State { get; set; } = EntityState.Detached;

        public EntityEntry(object entity)
        {
            Entity = entity;
        }
    }

    public class EntityEntry<T> : EntityEntry
    {
        public new T Entity { get; }

        public EntityEntry(T entity) 
            : base(entity)
        {
            Entity = entity;
        }
    }
}
