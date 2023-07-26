using System;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Infrastructure
{
    public class EntityTypeBuilder
    {
        private readonly EntityType _entityType;

        public EntityTypeBuilder(EntityType entityType)
        {
            _entityType = entityType;
        }

        public void HasKey(string property)
        {
            throw new NotImplementedException();
        }

        public PropertyBuilder Property(string property)
        {
            throw new NotImplementedException();
        }

        public void Build(EntityType entityType)
        {

        }
    }

    public class EntityTypeBuilder<T> : EntityTypeBuilder
    {
        public EntityTypeBuilder(EntityType entityType)
            : base(entityType)
        {

        }

        public void HasKey<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            throw new NotImplementedException();
        }

        public PropertyBuilder Property<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
