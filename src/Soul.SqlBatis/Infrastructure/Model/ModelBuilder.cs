using System;
using System.Collections.Concurrent;

namespace Soul.SqlBatis.Infrastructure
{
    public class ModelBuilder
	{
		private readonly ConcurrentDictionary<Type, EntityType> _entityTypes
			= new ConcurrentDictionary<Type, EntityType>();

		private static ConcurrentDictionary<Type, Model> _contextModels = new ConcurrentDictionary<Type, Model>();

		public EntityTypeBuilder Entity(Type type)
		{
			return new EntityTypeBuilder(GetEntityType(type));
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
				return new EntityType(key);
			});
		}

		public static Model CreateModel(Type type, Action<ModelBuilder> configure)
		{
			if (!typeof(DbContext).IsAssignableFrom(type))
			{
				throw new InvalidCastException(string.Format("{0} must derive from DbContext", type.Name));
			}
			return _contextModels.GetOrAdd(type, key =>
			{
				var modelBuilder = new ModelBuilder();
				configure(modelBuilder);
				return modelBuilder.Build();
			});
		}
	}
}
