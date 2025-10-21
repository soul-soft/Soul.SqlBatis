using System.Reflection;

namespace Soul.SqlBatis.Metadata
{

    internal class Property : Annotatable, IProperty
    {
        public Property(IEntityType entityType, PropertyInfo propertyInfo)
        {
            EntityType = entityType;
            PropertyInfo = propertyInfo;
            ValueGenerated = ValueGenerated.Never;
        }

        public string Name => PropertyInfo.Name;

        public IEntityType EntityType { get; private set; }

        public ValueGenerated ValueGenerated { get; private set; }

        public PropertyInfo PropertyInfo { get; private set; }

        internal void SetValueGenerated(ValueGenerated generated)
        {
            ValueGenerated = generated;
        }
    }
}
