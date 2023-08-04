using System;
using System.Linq.Expressions;

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

        public void HasColumnName(string name)
        { 

        }
    }
}
