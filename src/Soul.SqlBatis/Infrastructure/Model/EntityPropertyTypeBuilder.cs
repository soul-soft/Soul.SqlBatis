using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Infrastructure
{
    public class EntityPropertyTypeBuilder
	{
		private readonly EntityPropertyType _property;

		public EntityPropertyTypeBuilder(IEntityPropertyType property)
		{
			_property = property as EntityPropertyType;
		}

		public void HasColumnName(string name)
		{
			_property.SetAnnotation(new ColumnAttribute(name));
		}

		public void HasAnnotation(object annotation)
		{
			_property.SetAnnotation(annotation);
		}

		public void ValueGeneratedNever()
		{
			_property.RemoveAnnotation<IdentityAttribute>();
		}
	}

	public class EntityPropertyBuilder<T> : EntityPropertyTypeBuilder
		where T : class
	{
		public EntityPropertyBuilder(IEntityPropertyType property)
			: base(property)
		{

		}
	}
}
