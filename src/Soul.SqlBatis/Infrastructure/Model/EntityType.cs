using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public class EntityType
	{
		public Type Type { get; }

		public IAttributeCollection Attributes { get; } = new AttributeCollection();

		public IReadOnlyCollection<EntityProperty> Properties { get; }

		public IReadOnlyCollection<EntityProperty> KeyProperties => Properties.Where(a => a.IsKey).ToList();

		public string Schema
		{
			get
			{
				var name = Attributes.Get<TableAttribute>()?.Schema;
				return name ?? string.Empty;
			}
		}

		public virtual string TableName
		{
			get
			{
				var name = Attributes.Get<TableAttribute>()?.Name;
				if (!string.IsNullOrEmpty(name))
				{
					return name;
				}
				return Type.Name;
			}

		}

		public EntityType(Type type, IAttributeCollection attributes, List<EntityProperty> properties)
		{
			Type = type;
			Attributes = attributes;
			Properties = properties;
		}

		public virtual EntityProperty GetProperty(MemberInfo member)
		{
			return Properties.Where(a => a.Member == member).FirstOrDefault();
		}
	}
}
