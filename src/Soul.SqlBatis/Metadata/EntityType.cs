using System;
using System.Collections.Generic;
using System.Reflection;

namespace Soul.SqlBatis.Metadata
{
    public class EntityType : Annotatable, IEntityType
    {
        private readonly Dictionary<string, IProperty> _properties = new Dictionary<string, IProperty>();

        public EntityType(IModel model, Type typeInfo)
        {
            Model = model;
            TypeInfo = typeInfo;
        }

        public string Name => TypeInfo.Name;

        public Type TypeInfo { get; private set; }

        public IModel Model { get; private set; }

        public IKey PrimaryKey { get; private set; }

        public IProperty FindProperty(string name)
        {
            _properties.TryGetValue(name, out var property);
            return property;
        }

        public IEnumerable<IProperty> GetProperties()
        {
            return _properties.Values;
        }

        internal void SetPrimaryKey(IKey primaryKey)
        {
            PrimaryKey = primaryKey;
        }

        internal Property AddProperty(PropertyInfo propertyInfo)
        {
            var property = new Property(this, propertyInfo);
            _properties.Add(property.Name, property);
            return property;
        }
    }
}
