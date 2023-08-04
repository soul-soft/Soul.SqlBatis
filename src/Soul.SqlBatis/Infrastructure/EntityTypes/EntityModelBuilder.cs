using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    public class EntityModelBuilder
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

        public EntityModel Build()
        {
            return new EntityModel(_entityTypes.Values);
        }

        private EntityType GetEntityType(Type type)
        {
            return _entityTypes.GetOrAdd(type, key =>
            {
                return new EntityType(type);
            });
        }

        private static ConcurrentDictionary<Type, EntityModel> _modelCache = new ConcurrentDictionary<Type, EntityModel>();

        public static EntityModel CreateModel(Type type, Action<EntityModelBuilder> configure)
        {
            return _modelCache.GetOrAdd(type, key =>
            {
                var modelBuilder = new EntityModelBuilder();
                var entityTypes = key.GetProperties()
                   .Where(a => a.PropertyType.IsGenericType)
                   .Where(a => a.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));
                foreach (var item in entityTypes)
                {
                    var entityType = item.PropertyType.GenericTypeArguments[0];
                    modelBuilder.Entity(entityType).ToTable(item.PropertyType.Name);
                }
                configure(modelBuilder);
                return modelBuilder.Build();
            });
        }
    }
}
