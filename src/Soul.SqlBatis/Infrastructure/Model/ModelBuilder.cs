using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
	public class ModelBuilder
	{
		private readonly ConcurrentDictionary<Type, EntityType> _entityTypes 
			= new ConcurrentDictionary<Type, EntityType>();

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

		private static ConcurrentDictionary<Type, Model> _modelCache = new ConcurrentDictionary<Type, Model>();

		public static Model CreateModel(Type type, Action<ModelBuilder> configure)
		{
			return _modelCache.GetOrAdd(type, key =>
			{
				var modelBuilder = new ModelBuilder();
				configure(modelBuilder);
				return modelBuilder.Build();
			});
		}
	}
}
