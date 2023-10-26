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

        public object OriginalValue { get; internal set; }

		public bool IsModified
		{
			get
			{
				if (CurrentValue == null && OriginalValue == null)
				{
					_isModified = false;
				}
				else if (CurrentValue != null)
				{
					_isModified = !CurrentValue.Equals(OriginalValue);
				}
				else
				{
					_isModified = !OriginalValue.Equals(CurrentValue);
				}
				return _isModified.Value;
			}
			set 
			{
				_isModified = value;
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
