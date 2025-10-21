using Soul.SqlBatis.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soul.SqlBatis.ChangeTracking
{

    public class EntityEntry<T> : IEntityEntry
    {
        private readonly IEntityEntry _entityEntry;

        internal EntityEntry(IEntityEntry entityEntry)
        {
            _entityEntry = entityEntry;
        }

        public object Entity => _entityEntry.Entity;

        public IEntityType Metadata => _entityEntry.Metadata;

        public EntityState State { get => _entityEntry.State; set => _entityEntry.State = value; }

        public IReadOnlyList<PropertyEntry> Properties => _entityEntry.Properties;

        public bool IsPersisted()
        {
            return _entityEntry.IsPersisted();
        }

        public void SetCurrentValue(IProperty property, object value)
        {
            _entityEntry.SetCurrentValue(property, value);
        }
    }
}
