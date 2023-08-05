using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public class EntityProperty
	{
		public MemberInfo Member { get; }

		public IAnnotationCollection Annotations { get; } = new AnnotationCollection();

		public string ColumnName
		{
			get
			{
				var columnName = Annotations.Get<ColumnAttribute>()?.Name;
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
		
		public EntityProperty(MemberInfo member, IAnnotationCollection annotations)
		{
			Member = member;
			Annotations = annotations;
		}
	}
}
