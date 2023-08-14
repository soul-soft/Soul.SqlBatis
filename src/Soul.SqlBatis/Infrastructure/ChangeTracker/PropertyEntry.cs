using System.Collections.Generic;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public class PropertyEntry : IEntityProperty
    {
        public object Entity { get; }
     
        private IEntityProperty _entityProperty;

        public PropertyEntry(IEntityProperty property, object entity, object originalValue)
        {
            _entityProperty = property;
            Entity = entity;
            OriginalValue = originalValue;
        }

        public object CurrentValue => _entityProperty.Property.GetValue(Entity);

        public object OriginalValue { get; }

        public bool IsModified
        {
            get
            {
                if (ReferenceEquals(CurrentValue, OriginalValue))
                {
                    return false;
                }
                if (CurrentValue == null && OriginalValue == null)
                {
                    return false;
                }
                if (CurrentValue != null)
                {
                    return !CurrentValue.Equals(OriginalValue);
                }
                if (OriginalValue != null)
                {
                    return !OriginalValue.Equals(CurrentValue);
                }
                return CurrentValue != OriginalValue;
            }
        }

        public bool IsKey => _entityProperty.IsKey;

        public bool IsIdentity => _entityProperty.IsIdentity;

        public bool IsNotMapped => _entityProperty.IsNotMapped;

        public PropertyInfo Property => _entityProperty.Property;

        public string ColumnName => _entityProperty.ColumnName;

        public IReadOnlyCollection<object> Metadata => _entityProperty.Metadata;
    }
}
