using System;
using Soul.SqlBatis.Linq;

namespace Soul.SqlBatis.Model
{
    public class ModelBuilder
    {
        public IEntityBuilder<T> Entity<T>()
        {
            throw new NotImplementedException();
        }
        
        public EntityBuilder Entity(Type type)
        {
            return new EntityBuilder(type);
        }

        public IModel Build()
        { 
            throw new NotImplementedException();
        }
    }
}
