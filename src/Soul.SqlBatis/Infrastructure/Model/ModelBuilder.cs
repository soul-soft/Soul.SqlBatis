using System;
using System.Collections.Concurrent;

namespace Soul.SqlBatis.Infrastructure
{
    public class ModelBuilder
	{
		private readonly ConcurrentDictionary<Type, IEntityType> _entityTypes
			= new ConcurrentDictionary<Type, IEntityType>();

		public EntityTypeBuilder Entity(Type type)
		{
			return new EntityTypeBuilder(GetEntityType(type));
		}

		public EntityTypeBuilder<T> Entity<T>()
			where T : class
		{
			return new EntityTypeBuilder<T>(GetEntityType(typeof(T)));
		}

		public ConcurrentDictionary<Type, IEntityType> Build()
		{
			return _entityTypes;
		}

		private IEntityType GetEntityType(Type type)
		{
			return _entityTypes.GetOrAdd(type, key =>
			{
				return new EntityType(key);
			});
		}
	}
}
