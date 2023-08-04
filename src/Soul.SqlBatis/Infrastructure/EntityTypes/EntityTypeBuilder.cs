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

        public EntityTypePropertyBuilder Property(string property)
        {
            return EntityTypePropertyBuilder();
        }

        public EntityTypePropertyBuilder Property<T>(string property)
        {
            return EntityTypePropertyBuilder<T>();
        }

        public void ToTable(string name, string scheme = null)
        {
            _entityType.TableName = name;
            _entityType.Scheme = scheme;
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

        public EntityTypePropertyBuilder Property<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
