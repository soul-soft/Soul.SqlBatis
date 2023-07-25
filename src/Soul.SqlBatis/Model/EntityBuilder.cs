using System;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Model
{
    public class EntityBuilder
    {
        public Type EntityType { get; }

        public EntityBuilder(Type entityType)
        {
            EntityType = entityType;
        }

        public void HasKey(string property)
        {
            throw new NotImplementedException();
        }

        public IPropertyBuilder Property<TProperty>(string property)
        {
            throw new NotImplementedException();
        }
    }

    public class EntityBuilder<T> : EntityBuilder
    {
        public EntityBuilder() 
            : base(typeof(T))
        {

        }

        public void HasKey<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            throw new NotImplementedException();
        }

        public IPropertyBuilder Property<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
