using System.Collections.Generic;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	internal class EntityPropertyEntryCache : IEntityPropertyEntry
	{
		private readonly IEntityPropertyType _propertyType;
		
		public object CurrentValue { get; }

		public object OriginalValue { get; }

		public bool IsModified { get; }
		
		public EntityPropertyEntryCache(IEntityPropertyType propertyType,object currentValue,object originalValue,bool isModified)
		{
			_propertyType = propertyType;
			CurrentValue = currentValue;
			OriginalValue = originalValue;
			IsModified = isModified;
		}

		public bool IsKey => _propertyType.IsKey;

		public bool IsIdentity => _propertyType.IsIdentity;

		public bool IsNotMapped => _propertyType.IsNotMapped;

		public bool IsConcurrencyToken => _propertyType.IsConcurrencyToken;

		public PropertyInfo Property => _propertyType.Property;

		public string ColumnName => _propertyType.ColumnName;

		public string CSharpName => _propertyType.CSharpName;

		public IReadOnlyCollection<object> Metadata => _propertyType.Metadata;

		
	}
}
