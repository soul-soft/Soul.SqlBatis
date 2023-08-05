using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public class EntityPropertyBuilder
	{
		private MemberInfo _member;

		public IAttributeCollection _annotations;

		public EntityPropertyBuilder(MemberInfo member)
		{
			_member = member;
			_annotations = new AttributeCollection();
		}

		public EntityPropertyBuilder(EntityPropertyBuilder target)
		{
			_member = target._member;
			_annotations = target._annotations;
		}

		public void HasColumnName(string name)
		{
			_annotations.Set(new ColumnAttribute(name));
		}

		public void HasAnnotation(object annotation)
		{
			_annotations.Set(annotation);
		}

		public EntityProperty Build()
		{
			return new EntityProperty(_member, _annotations);
		}

	}

	public class EntityPropertyBuilder<T> : EntityPropertyBuilder
		where T : class
	{
		public EntityPropertyBuilder(EntityPropertyBuilder target)
			: base(target)
		{

		}
	}
}
