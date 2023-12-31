﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
	public class ChangeTracker
	{
		private readonly IModel _model;

		private readonly Dictionary<object, IEntityEntry> _entryReferences = new Dictionary<object, IEntityEntry>();

		public ChangeTracker(IModel model)
		{
			_model = model;
		}

		public IEnumerable<IEntityEntry> Entries()
		{
			return _entryReferences.Values;
		}

		public IEnumerable<IEntityEntry<T>> Entries<T>()
		{
			return _entryReferences.Values
				.Where(a => a.GetType() == typeof(T))
				.Select(s => new EntityEntry<T>(s));
		}

		public bool HasEntry(object entry)
		{
			return _entryReferences.Keys.Any(a => ReferenceEquals(a, entry));
		}

		public IEntityEntry Find(Type type, object key)
		{
			var property = _model.GetEntityType(type).Properties
				.Where(a => a.IsKey)
				.Select(s => s.Property)
				.First();
			var changeKey = Convert.ChangeType(key, property.PropertyType);
			foreach (var entry in _entryReferences.Values.Where(a => a.Type == type))
			{
				var cacheKey = entry.Values
					.Where(a => a.IsKey)
					.Select(s => s.OriginalValue)
					.First();
				if (cacheKey.Equals(changeKey))
				{
					return entry;
				}
			}
			return null;
		}

		public IEntityEntry<T> TrackGraph<T>(T entity)
		{
			var entry = GetOrCreateEntry(entity);
			return new EntityEntry<T>(entry);
		}

		private IEntityEntry GetOrCreateEntry(object entity)
		{
			if (entity == null)
			{
				throw new InvalidOperationException("Unable to track null pointer");
			}
			if (_entryReferences.ContainsKey(entity))
			{
				return _entryReferences[entity];
			}
			return CreateEntry(entity);
		}

		private IEntityEntry CreateEntry(object entity)
		{
			var entityType = _model.GetEntityType(entity.GetType());
			var func = TypeMapper.CreateDeserializer(entity.GetType());
			var values = func(entity);
			var properties = entity.GetType().GetProperties()
				.Select(property => new EntityPropertyEntry(entityType.GetProperty(property), entity, values[property.Name]))
				.ToList();
			var entry = new EntityEntry(entityType, entity, properties)
			{
				State = EntityState.Unchanged
			};
			_entryReferences.Add(entity, entry);
			return entry;
		}

		internal void Clear()
		{
			_entryReferences.Clear();
		}
	}
}
