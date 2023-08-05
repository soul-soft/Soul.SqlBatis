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

		public IAttributeCollection Attributes { get; } = new AttributeCollection();

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
		}
		
		public EntityProperty(MemberInfo member, IAttributeCollection annotations)
		{
			Member = member;
			Attributes = annotations;
		}
	}
}
