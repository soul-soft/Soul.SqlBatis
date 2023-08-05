using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    public class ModelBuilder
    {
        private readonly ConcurrentDictionary<Type, EntityTypeBuilder> _entityTypeBuilders = new ConcurrentDictionary<Type, EntityTypeBuilder>();

        public EntityTypeBuilder Entity(Type type)
        {
            return GetEntityTypeBuilder(type);
        }

        public EntityTypeBuilder<T> Entity<T>()
            where T : class
        {
            var target = GetEntityTypeBuilder(typeof(T));
			return new EntityTypeBuilder<T>(target);
        }

        public Model Build()
        {
            return new Model();
        }

        private EntityTypeBuilder GetEntityTypeBuilder(Type type)
        {
            return _entityTypeBuilders.GetOrAdd(type, key =>
            {
                return new EntityTypeBuilder(type);
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
