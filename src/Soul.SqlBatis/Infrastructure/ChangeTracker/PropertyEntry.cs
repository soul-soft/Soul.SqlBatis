using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public class PropertyEntry : IEntityProperty
	{
		public object Entity { get; }

		private readonly IEntityProperty _property;

		private object _currentValue;

		private bool? _isModified;

		public PropertyEntry(IEntityProperty property, object entity, object originalValue)
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

		[DebuggerHidden]
		public object CurrentValue
		{
			get
			{
				if (_currentValue == null)
				{
					_currentValue = _property.Property.GetValue(Entity);
				}
				return _currentValue;
			}
		}

		public object OriginalValue { get; }

		public bool IsModified
		{
			get
			{
				if (_isModified != null)
				{
					return _isModified.Value;
				}
				if (CurrentValue == null && OriginalValue == null)
				{
					_isModified = false;
					return false;
				}
				if (CurrentValue != null)
				{
					_isModified = !CurrentValue.Equals(OriginalValue);
				}
				else
				{
					_isModified = !OriginalValue.Equals(CurrentValue);
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
	}
}
