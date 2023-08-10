using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public class PropertyEntry
	{
		public object Entity { get; }
		public PropertyInfo Member { get; }
		public object CurrentValue => Member.GetValue(Entity);
		public object OriginalValue { get; }

		public PropertyEntry(object entity, PropertyInfo member)
		{
			Entity = entity;
			Member = member;
			OriginalValue = member.GetValue(entity);
		}

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
	}

	public class PropertyEntry<TEntity, TProperty> : PropertyEntry
	{
		public PropertyEntry(PropertyEntry property)
			: base(property.Entity, property.Member)
		{
		}
	}
}
