using System;
using System.Collections.Concurrent;

namespace Soul.SqlBatis.Infrastructure
{
    public class ModelBuilder
    {
        private readonly ConcurrentDictionary<Type, EntityType> _entityTypes = new ConcurrentDictionary<Type, EntityType>();

        public EntityTypeBuilder Entity(Type type)
        {
            return new EntityTypeBuilder(GetEntityType(type));
        }

        public EntityTypeBuilder Entity<T>()
        {
            return new EntityTypeBuilder<T>(GetEntityType(typeof(T)));
        }

        public Model Build()
        {
            return new Model(_entityTypes.Values);
        }

        private EntityType GetEntityType(Type type)
        {
            return _entityTypes.GetOrAdd(type, key =>
            {
                return new EntityType(type);
            });
        }
    }
}
