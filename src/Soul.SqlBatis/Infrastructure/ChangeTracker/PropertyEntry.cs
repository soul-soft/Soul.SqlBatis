using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
	}

	public class PropertyEntry<TEntity, TProperty> : PropertyEntry
	{
		public PropertyEntry(PropertyEntry property)
			: base(property.Entity, property.Member)
		{
		}
	}
}
