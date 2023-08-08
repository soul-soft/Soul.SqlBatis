using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Infrastructure
{
	public class EntityPropertyBuilder
	{
		private readonly EntityProperty _property;

		public EntityPropertyBuilder(EntityProperty property)
		{
			_property = property;
		}

		public void HasColumnName(string name)
		{
			_property.HasAnnotation(new ColumnAttribute(name));
		}

		public void HasAnnotation(object annotation)
		{
			_property.HasAnnotation(annotation);
		}

		public void ValueGeneratedNever()
		{
			_property.RemoveAnnotation<IdentityAttribute>();
		}
	}

	public class EntityPropertyBuilder<T> : EntityPropertyBuilder
		where T : class
	{
		public EntityPropertyBuilder(EntityProperty property)
			: base(property)
		{

		}
	}
}
