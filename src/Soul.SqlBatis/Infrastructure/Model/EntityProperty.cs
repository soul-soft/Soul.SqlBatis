using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public class EntityProperty
	{
		public MemberInfo Member { get; }

		public IAnnotationCollection Annotations { get; } = new AnnotationCollection();

		private string _columnName;

		public string ColumnName
		{
			get
			{
				return _columnName;
			}
			set
			{
				_columnName = value;
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
