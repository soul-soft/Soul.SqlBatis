using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public class EntityPropertyBuilder
	{
		public MemberInfo Member { get; }

		public IAnnotationCollection Annotations { get; } = new AnnotationCollection();

		public EntityPropertyBuilder(MemberInfo member)
		{
			Member = member;
		}

		public void Ignore()
		{
			HasAnnotation(new NotMappedAttribute());
		}

		public void HasColumnName(string name)
		{
			Annotations.Set(new ColumnAttribute(name));
		}

		public void HasAnnotation(object annotation)
		{
			Annotations.Set(annotation);
		}

		public EntityProperty Build()
		{
			return new EntityProperty(Member, Annotations);
		}

	}

	public class EntityPropertyBuilder<T> : EntityPropertyBuilder
		where T : class
	{
		private readonly EntityPropertyBuilder _target;

		public EntityPropertyBuilder(EntityPropertyBuilder target)
			: base(target.Member)
		{
			_target = target;
		}
	}
}
