using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public class EntityPropertyEntry : IEntityPropertyType
	{
		public object Entity { get; }

		private readonly IEntityPropertyType _property;

		private object _currentValueCache;

		private bool? _isModified;

		public EntityPropertyEntry(IEntityPropertyType property, object entity, object originalValue)
		{
			_property = property;
			Entity = entity;
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
			_isModified = true;
		}

		private void NotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			_isModified = true;
		}

		public object CurrentValue
		{
			get
			{
				return _property.Property.GetValue(Entity);
			}
		}

		private bool _hasCurrentValueCache = false;

        internal object CurrentValueCache
        {
            get
            {
                if (!_hasCurrentValueCache)
                {
                    _currentValueCache = CurrentValue;
					_hasCurrentValueCache = true;
                }
                return _currentValueCache;
            }
        }

        public object OriginalValue { get; }

		internal bool IsModified
		{
			get
			{
				if (_isModified != null)
				{
					return _isModified.Value;
				}
				if (CurrentValueCache == null && OriginalValue == null)
				{
					_isModified = false;
					return false;
				}
				if (CurrentValueCache != null)
				{
					_isModified = !CurrentValueCache.Equals(OriginalValue);
				}
				else
				{
					_isModified = !OriginalValue.Equals(CurrentValueCache);
				}
				return _isModified.Value;
			}
		}

		public bool IsKey => _property.IsKey;

		public bool IsIdentity => _property.IsIdentity;

		public bool IsNotMapped => _property.IsNotMapped;

		public PropertyInfo Property => _property.Property;

		public string ColumnName => _property.ColumnName;

		public IReadOnlyCollection<object> Metadata => _property.Metadata;

        public string CSharpName => _property.CSharpName;
    }
}
