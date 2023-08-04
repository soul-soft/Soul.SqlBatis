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
        public void Ignore<T>()
          where T : class
        {

        }

        public EntityTypePropertyBuilder Property(string property)
        {
            return new EntityTypePropertyBuilder();
        }


        public void ToTable(string name, string scheme = null)
        {
            _entityType.TableName = name;
            _entityType.Scheme = scheme;
        }

        public void ToView(string name, string scheme = null)
        {
            _entityType.TableName = name;
            _entityType.Scheme = scheme;
        }

        public void Build(EntityType entityType)
        {

        }
    }

    public class EntityTypeBuilder<T> : EntityTypeBuilder
        where T : class
    {
        public EntityTypeBuilder(EntityType entityType)
            : base(entityType)
        {

        }
       
        public void Ignore<TProperty>(Expression<Func<T, TProperty>> expression)
        {

        }

        public void HasKey<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            throw new NotImplementedException();
        }

        public EntityTypePropertyBuilder<T> Property<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            return new EntityTypePropertyBuilder<T>(expression.Body);
        }
    }
}
