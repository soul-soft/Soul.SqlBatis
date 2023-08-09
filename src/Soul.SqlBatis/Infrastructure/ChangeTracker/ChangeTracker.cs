using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
	public class ChangeTracker
	{
		private readonly Dictionary<object, EntityEntry> _entryReferences = new Dictionary<object, EntityEntry>();

		public IEnumerable<EntityEntry> Entries()
		{
			return _entryReferences.Values;
		}

		public EntityEntry TrackGraph(object entity)
		{
			return GetOrCreateEntry(entity);
		}

		public EntityEntry<T> TrackGraph<T>(T entity)
		{
			var entry = GetOrCreateEntry(entity);
			return new EntityEntry<T>(entry);
		}

		private EntityEntry GetOrCreateEntry(object entity)
		{
			if (_entryReferences.ContainsKey(entity))
			{
				return _entryReferences[entity];
			}
			return CreateEntry(entity);
		}

		private EntityEntry CreateEntry(object entity)
		{
			var properties = entity.GetType().GetProperties()
				.Select(s => new PropertyEntry(entity, s))
				.ToList();
			var entry = new InternalEntityEntry(entity, properties);
			_entryReferences.Add(entity, entry);
			return entry;
		}

		private EntityEntry<T> CreateEntry<T>(T entity)
		{
			var entry = CreateEntry(entity);
			return new EntityEntry<T>(entry);
		}

		internal void Clear()
		{
			_entryReferences.Clear();
		}
	}
}
