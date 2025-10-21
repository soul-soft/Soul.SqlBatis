using Soul.SqlBatis.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.ChangeTracking
{
    public interface IChangeTracker
    {
        IEnumerable<EntityEntry> Entities();
        IEnumerable<EntityEntry<T>> Entities<T>() where T : class;
        EntityEntry<T> Track<T>(T entity);
        bool HasEntry<T>(T entity);
    }

    public class ChangeTracker : IChangeTracker
    {
        private readonly IModel _model;

        private readonly Dictionary<object, EntityEntry> _references = new Dictionary<object, EntityEntry>();

        public ChangeTracker(IModel model)
        {
            _model = model;
        }

        public IEnumerable<EntityEntry> Entities()
        {
            return _references.Select(s => s.Value);
        }

        public IEnumerable<EntityEntry<T>> Entities<T>() where T : class
        {
            return _references.Values
                .Where(a => a.Entity is T)
                .Select(s => new EntityEntry<T>(s));
        }

        public bool HasEntry<T>(T entity)
        {
            return _references.ContainsKey(entity);
        }

        public EntityEntry<T> Track<T>(T entity)
        {
            if (!HasEntry(entity))
            {
                var entityEntry = CreateEntityEntry(entity);
                _references.Add(entity, entityEntry);
            }
            var entry = _references[entity];
            return new EntityEntry<T>(entry);
        }


        private EntityEntry CreateEntityEntry(object entity)
        {
            var metadata = _model.FindEntityType(entity.GetType());
            var entityEntry = new EntityEntry(entity, metadata);
            foreach (var item in metadata.GetProperties())
            {
                entityEntry.AddProperty(item);
            }
            if (entityEntry.IsPersisted())
            {
                entityEntry.State = EntityState.Unchanged;
            }
            else 
            {
                entityEntry.State = EntityState.Added;
            }
            return entityEntry;
        }
    }
}
