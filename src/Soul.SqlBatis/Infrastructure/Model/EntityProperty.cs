using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public class EntityProperty
	{
		public MemberInfo Member { get; }

		public bool IsKey
		{
			get
			{
				return Attributes.Any(a => a is KeyAttribute);
			}
		}

		public bool IsNotMapped
		{
			get
			{
				return Attributes.Any(a => a is NotMappedAttribute);
			}
		}

		public IAttributeCollection Attributes { get; }

		public string ColumnName
		{
			get
			{
				var columnName = Attributes.Get<ColumnAttribute>()?.Name;
				if (!string.IsNullOrEmpty(columnName))
				{
					return columnName;
				}
				return Member.Name;
			}
		}

		public EntityProperty(MemberInfo member)
		{
			Member = member;
			Attributes = new AttributeCollection(member.GetCustomAttributes());
		}

		public void HasAnnotation(object value)
		{
			Attributes.Set(value);
		}
	}
}
