using Soul.SqlBatis.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.ChangeTracking
{
    public interface IChangeTracker
    {
        EntityEntry Entry(object entity);
        IEnumerable<EntityEntry> Entities();
        void Track(EntityEntry entry);
        void UnTrack(object entity);
        bool HasEntry(object entity);
    }

    public class ChangeTracker : IChangeTracker
    {
        private readonly DbContext _context;

        private readonly Dictionary<object, EntityEntry> _references = new Dictionary<object, EntityEntry>();

        public ChangeTracker(DbContext context)
        {
            _context = context;
        }

        public IEnumerable<EntityEntry> Entities()
        {
            return _references.Select(s => s.Value);
        }

        public bool HasEntry(object entity)
        {
            return _references.ContainsKey(entity);
        }

        public void Track(EntityEntry entry)
        {
            if (_references.TryGetValue(entry.Entity, out var oldEntry))
            {
                if (!ReferenceEquals(entry.Entity, oldEntry.Entity))
                {
                    throw new NotSupportedException("Cannot track entity: another instance with the same key is already being tracked.");
                }
            }
            else
            {
                _references[entry.Entity] = entry;
            }
        }

        public void Track(object entity, EntityState state)
        {
            var entry = Entry(entity);
            entry.State = state;
        }

        public void UnTrack(object entity)
        {
            _references.Remove(entity);
        }

        public EntityEntry Entry(object entity)
        {
            if (_references.TryGetValue(entity, out var entry))
            {
                return entry;
            }
            var metadata = _context.Model.FindEntityType(entity.GetType());
            var entityEntry = new EntityEntry(_context, entity, metadata);
            return entityEntry;
        }
    }
}
