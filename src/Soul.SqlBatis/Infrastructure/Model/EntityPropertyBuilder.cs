using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public class EntityPropertyBuilder
	{
		public string GuidS => Guid.NewGuid().ToString();
		public MemberInfo Member { get; }

		public virtual IAnnotationCollection Annotations { get; } = new AnnotationCollection();

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

		public override IAnnotationCollection Annotations => _target.Annotations;

		public EntityPropertyBuilder(EntityPropertyBuilder target)
			: base(target.Member)
		{
			_target = target;
		}
	}
}
