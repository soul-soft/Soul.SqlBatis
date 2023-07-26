using System;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Infrastructure
{
    public class EntityTypeBuilder
    {
        private readonly Type _type;

        public EntityTypeBuilder(Type type)
        {
            _type = type;
        }

        public void HasKey(string property)
        {
            throw new NotImplementedException();
        }

        public PropertyBuilder Property(string property)
        {
            throw new NotImplementedException();
        }

        public EntityType Build()
        {
            return new EntityType(null);
        }
    }

    public class EntityTypeBuilder<T> : EntityTypeBuilder
    {
        public EntityTypeBuilder()
            : base(typeof(T))
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
