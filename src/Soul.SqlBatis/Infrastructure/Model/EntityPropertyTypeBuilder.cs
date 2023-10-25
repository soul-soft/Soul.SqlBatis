using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Infrastructure
{
    public class EntityPropertyBuilder
	{
		private readonly EntityPropertyType _property;

		public EntityPropertyBuilder(IEntityProperty property)
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

	public class EntityPropertyBuilder<T> : EntityPropertyBuilder
		where T : class
	{
		public EntityPropertyBuilder(IEntityProperty property)
			: base(property)
		{

		}
	}
}
