using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public class EntityTypePropertyBuilder
    {

    }

    public class EntityTypePropertyBuilder<T>
        where T : class
    {
        public EntityTypePropertyBuilder(Expression expression)
        {

        }

        public void HasKey<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
