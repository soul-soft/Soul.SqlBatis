using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    public class PropertyEntry : IEntityProperty
    {
        public object Entity { get; }

        private IEntityProperty _entityProperty;

        private object _currentValue;

        public PropertyEntry(IEntityProperty property, object entity, object originalValue)
        {
            _entityProperty = property;
            Entity = entity;
            OriginalValue = originalValue;
        }

        [DebuggerHidden]
        public object CurrentValue
        {
            get
            {
                if (_currentValue == null)
                {
                    _currentValue = _entityProperty.Property.GetValue(Entity);
                }
                return _currentValue;
            }
        }

        public object OriginalValue { get; }

        public bool IsModified
        {
            get
            {
                if (CurrentValue == null && OriginalValue == null)
                {
                    return false;
                }
                if (CurrentValue != null)
                {
                    return !CurrentValue.Equals(OriginalValue);
                }
                return !OriginalValue.Equals(CurrentValue);
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
