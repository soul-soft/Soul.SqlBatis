using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    public class ModelBuilder
    {
        private readonly ConcurrentDictionary<Type, EntityType> _entityTypes = new ConcurrentDictionary<Type, EntityType>();

        public EntityTypeBuilder Entity(Type type)
        {
            return new EntityTypeBuilder(GetEntityType(type));
        }

        public void Ignore<T>()
            where T : class
        { 

        }

        public EntityTypeBuilder<T> Entity<T>()
            where T : class
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

        private static ConcurrentDictionary<Type, Model> _modelCache = new ConcurrentDictionary<Type, Model>();

        public static Model CreateModel(Type type, Action<ModelBuilder> configure)
        {
            return _modelCache.GetOrAdd(type, key =>
            {
                var modelBuilder = new ModelBuilder();
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
