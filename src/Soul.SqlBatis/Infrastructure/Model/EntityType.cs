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

		public IAnnotationCollection Annotations { get; } = new AnnotationCollection();

		public virtual IReadOnlyCollection<EntityProperty> Properties { get; }


		public string Scheme
		{
			get
			{
				var name = Annotations.Get<TableAttribute>()?.Schema;
				return name ?? string.Empty;
			}
		}

		public virtual string TableName
		{
			get
			{
				var name = Annotations.Get<TableAttribute>()?.Name;
				if (!string.IsNullOrEmpty(name))
				{
					return name;
				}
				return Type.Name;
			}

		}

		public EntityType(Type type, IAnnotationCollection annotations, List<EntityProperty> properties)
		{
			Type = type;
			Annotations = annotations;
			Properties = properties;
		}

		public virtual EntityProperty GetProperty(MemberInfo member)
		{
			return Properties.Where(a => a.Member == member).FirstOrDefault();
		}
	}
}
