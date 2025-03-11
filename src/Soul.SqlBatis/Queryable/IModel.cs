using Soul.SqlBatis.Databases;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis
{
    public interface IModel
    {
        string Format(string name);
        IEntityType FindEntityType(Type type);
    }

    public interface IEntityType
    {
        Type DeclaringType { get; }
        string TableName { get; }
        IEntityProperty GetProperty(string memberName);
        IReadOnlyList<IEntityProperty> GetProperties();
    }

    public interface IEntityProperty
    {
        PropertyInfo Property { get; }
        string ColumnName { get; }
        bool IsNotMapped();
        bool IsIdentity();
        bool IsKey();
    }

    internal class AnnotationModel : IModel
    {
        private readonly SqlSettings _options;

        private static readonly ConcurrentDictionary<Type, IEntityType> _entityTypes = new ConcurrentDictionary<Type, IEntityType>();

        public AnnotationModel(SqlSettings options)
        {
            _options = options;
        }

        public string Format(string name)
        {
            return string.Format(_options.IdentifierFormat, name);
        }

        public IEntityType FindEntityType(Type type)
        {
            return _entityTypes.GetOrAdd(type, (key) =>
            {
                var properties = type.GetProperties().Select(s => new AnnotationProperty(this, s));
                return new AnnotationEntityType(this, key, properties);
            });
        }
    }

    internal class AnnotationEntityType : IEntityType
    {
        private readonly IModel _model;
        private readonly IReadOnlyList<IEntityProperty> _properties;

        public AnnotationEntityType(IModel model, Type declaringType, IEnumerable<IEntityProperty> properties)
        {
            _model = model;
            DeclaringType = declaringType;
            _properties = properties.ToList();
        }

        public Type DeclaringType { get; private set; }

        public string TableName
        {
            get
            {
                var tableAttribute = GetAnnotation<TableAttribute>();
                var name = tableAttribute?.Name ?? DeclaringType.Name;
                var schema = tableAttribute?.Schema;
                if (string.IsNullOrEmpty(schema)) 
                {
                    return _model.Format(name);
                }
                else
                {
                    return $"{_model.Format(schema)}.{_model.Format(name)}";
                }
            }
        }

        public IReadOnlyList<IEntityProperty> GetProperties()
        {
            return _properties;
        }

        public IEntityProperty GetProperty(string memberName)
        {
            return _properties.Where(a => a.Property.Name == memberName).FirstOrDefault();
        }

        private TAnnotation GetAnnotation<TAnnotation>() where TAnnotation : Attribute
        {
            return DeclaringType.GetCustomAttribute<TAnnotation>();
        }
    }

    internal class AnnotationProperty : IEntityProperty
    {
        private readonly IModel _model;

        private readonly List<Attribute> _matedata;

        public PropertyInfo Property { get; private set; }

        public AnnotationProperty(IModel model, PropertyInfo property)
        {
            _model = model;
            Property = property;
            _matedata = property.GetCustomAttributes().ToList();
        }

        public string ColumnName
        {
            get
            {
                var name = GetAnnotation<ColumnAttribute>()?.Name ?? Property.Name;
                return _model.Format(name);
            }
        }

        public bool IsIdentity()
        {
            return HasAnnotation<IdentityAttribute>()
                && !HasAnnotation<NotIdentityAttribute>();
        }

        public bool IsNotMapped()
        {
            return HasAnnotation<NotMappedAttribute>();
        }

        public bool IsKey()
        {
            return HasAnnotation<KeyAttribute>();
        }

        private bool HasAnnotation<TAnnotation>() where TAnnotation : Attribute
        {
            return GetAnnotation<TAnnotation>() != null;
        }

        private TAnnotation GetAnnotation<TAnnotation>() where TAnnotation : Attribute
        {
            return _matedata.Where(a => a is TAnnotation).Cast<TAnnotation>().FirstOrDefault();
        }
    }
}
