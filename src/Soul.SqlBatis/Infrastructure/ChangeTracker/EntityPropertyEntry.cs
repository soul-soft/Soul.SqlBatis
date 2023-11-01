using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
    internal class EntityPropertyEntry : IEntityPropertyEntry
    {
        private object _entity;

        private readonly IEntityPropertyType _property;

        private bool? _referenceIsModified = false;

        public EntityPropertyEntry(IEntityPropertyType property, object entity, object originalValue)
        {
            _property = property;
            _entity = entity;
            OriginalValue = originalValue;
            ListenChange();
        }

        private void ListenChange()
        {
            if (OriginalValue != null && OriginalValue is INotifyCollectionChanged notifyCollectionChanged)
            {
                notifyCollectionChanged.CollectionChanged += NotifyCollectionChanged;
            }
            else if (OriginalValue != null && OriginalValue is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged += NotifyPropertyChanged;
            }
        }

        private void NotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _referenceIsModified = true;
        }

        private void NotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _referenceIsModified = true;
        }

        public object CurrentValue
        {
            get
            {
                return _property.Property.GetValue(_entity);
            }
        }

        public object OriginalValue { get; internal set; }

        public bool IsModified
        {
            get
            {
                return CalcModified(CurrentValue, OriginalValue);
            }
        }

        public bool CalcModified(object currentValue, object originalValue)
        {
            if (_referenceIsModified == true)
            {
                return true;
            }
            if (currentValue == null && originalValue == null)
            {
                return false;
            }
            else if (currentValue != null)
            {
                return !currentValue.Equals(originalValue);
            }
            else
            {
                return !OriginalValue.Equals(currentValue);
            }
        }

        public bool IsKey => _property.IsKey;

        public bool IsIdentity => _property.IsIdentity;

        public bool IsNotMapped => _property.IsNotMapped;

        public PropertyInfo Property => _property.Property;

        public string ColumnName => _property.ColumnName;

        public IReadOnlyCollection<object> Metadata => _property.Metadata;

        public string CSharpName => _property.CSharpName;

        public bool IsConcurrencyToken => _property.IsConcurrencyToken;
    }
}
