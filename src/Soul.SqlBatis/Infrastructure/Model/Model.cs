using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Soul.SqlBatis.Infrastructure
{
	public class Model
	{
		private readonly ConcurrentDictionary<Type, EntityType> _entities = new ConcurrentDictionary<Type, EntityType>();

		public Model(IEnumerable<EntityType> entities)
		{
			foreach (var item in entities)
			{
				_entities.TryAdd(item.Type, item);
			}
		}

		public virtual EntityType GetEntityType(Type type)
		{
			return _entities.GetOrAdd(type, key => 
			{
				return new EntityType(key);
			});
		}
	}
}
