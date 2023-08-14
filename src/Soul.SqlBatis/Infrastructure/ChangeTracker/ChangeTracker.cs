using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.Infrastructure
{
    public class ChangeTracker
    {
        private Model _model;

        private readonly Dictionary<object, EntityEntry> _entryReferences = new Dictionary<object, EntityEntry>();

        public ChangeTracker(Model model)
        {
            _model = model;
        }

        public IEnumerable<EntityEntry> Entries()
        {
            return _entryReferences.Values;
        }

        public IEnumerable<EntityEntry<T>> Entries<T>()
        {
            return _entryReferences.Values
                .Where(a => a.GetType() == typeof(T))
                .Select(s => new EntityEntry<T>(s));
        }

        public EntityEntry Find(Type type, object key)
        {
            var entityType = _model.GetEntityType(type);
            var keyProperty = entityType.Properties.Where(a => a.IsKey).First().Property;
            foreach (var entry in _entryReferences.Values.Where(a => a.Type == type))
            {
                var cacheKey = entry.Properties.Where(a => a.IsKey).Select(s=>s.OriginalValue).First();
                if (cacheKey.Equals(key))
                {
                    return entry;
                }
            }
            return null;
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
            var entityType = _model.GetEntityType(entity.GetType());
            var func = TypeSerializer.CreateDeserializer(entity.GetType());
            var values = func(entity);
            var properties = entity.GetType().GetProperties()
                .Select(property => new PropertyEntry(entityType.GetProperty(property), entity, values[property.Name]))
                .ToList();
            var entry = new EntityEntry(entity, entityType, properties);
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
