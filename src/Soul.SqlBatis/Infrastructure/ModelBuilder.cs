using System;
using System.Collections.Generic;

namespace Soul.SqlBatis.Infrastructure
{
    public class ModelBuilder
    {
        private readonly List<EntityTypeBuilder> _entities = new List<EntityTypeBuilder>();

        public EntityTypeBuilder Entity(Type type)
        {
            return new EntityTypeBuilder(type);
        }

        public Model Build()
        {
            throw new NotImplementedException();
        }

        private static EntityTypeBuilder Create(Type type)
        {
            return new EntityTypeBuilder(type);
        }
    }
}
