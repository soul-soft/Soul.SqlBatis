using Soul.SqlBatis.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Soul.SqlBatis.ChangeTracking
{
    public interface IEntityEntry
    {
        object Entity { get; }
        IEntityType Metadata { get; }
        EntityState State { get; set; }
        IReadOnlyList<PropertyEntry> Properties { get; }
        bool IsPersisted();
        void SetCurrentValue(IProperty property, object value);
    }
}
