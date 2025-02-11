using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.ChangeTracking
{
    public interface IChangeTracker
    {
        IEnumerable<EntityEntry> Entities();
        IEnumerable<EntityEntry<T>> Entities<T>() where T : class;
        EntityEntry<T> Track<T>(T entity, bool ignoreNullMembers = false);
        bool HasChanges();
        bool HasEntry<T>(T entity);
        void Untrack(object entity);
        void ClearEntities();
        IEnumerable<IEntityEntry> GetChangedEntries();
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

        public void Untrack(object entity)
        {
            _references.Remove(entity);
        }

        public void ClearEntities()
        {
            _references.Clear();
        }

        public EntityEntry<T> Track<T>(T entity, bool ignoreNullMembers = false)
        {
            if (!_references.ContainsKey(entity))
            {
                var mapper = DynamicParametersMapper.CreateMapper(typeof(T));
                var values = mapper(entity);
                var entityType = _model.FindEntityType(typeof(T));
                var members = new List<MemberEntry>();
                foreach (var item in entityType.GetProperties().Where(a => !a.IsNotMapped()))
                {
                    var value = values[item.Property.Name];
                    members.Add(new MemberEntry(entity, value, item));
                }
                var entityEntry = new EntityEntry(entity, ignoreNullMembers, entityType, members);
                _references.Add(entity, entityEntry);
            }
            var entry = _references[entity];
            return new EntityEntry<T>(entry);
        }

        public bool HasChanges()
        {
            return _references.Values.Any(e => e.IsChanged);
        }

        public IEnumerable<IEntityEntry> GetChangedEntries()
        {
            var entries = _references.Values.Select(entry =>
            {
                var members = entry.Members.Select(s =>
                {
                    var originalValue = s.OriginalValue;
                    var currentValue = s.CurrentValue;
                    return new MemberEntrySnapshot(s, !EntityEntry.ValueEquals(originalValue, currentValue), originalValue, currentValue);
                }).ToList();
                var state = entry.StateSnapshot;
                var isChanged = members.Any(a => a.IsChanged);
                if (state == EntityState.Unchanged && isChanged)
                {
                    state = EntityState.Modified;
                }
                else if (state == EntityState.Modified && !isChanged)
                {
                    state = EntityState.Unchanged;
                }
                return new EntityEntrySnapshot(entry, isChanged, state, members);
            }).ToList();

            return entries.Where(entry =>
                     entry.State == EntityState.Added ||
                     entry.State == EntityState.Modified ||
                     entry.State == EntityState.Deleted).ToList();
        }
    }
}
