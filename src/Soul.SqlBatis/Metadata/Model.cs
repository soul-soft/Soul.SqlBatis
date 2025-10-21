using Soul.SqlBatis.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Soul.SqlBatis.Metadata
{
    internal class Model : IModel
    {
        private readonly SqlSettings _options;

        private static readonly ConcurrentDictionary<Type, IEntityType> _entityTypes = new ConcurrentDictionary<Type, IEntityType>();

        public Model(SqlSettings options)
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
                var entityType = new EntityType(this, key);
                var tableAttribute = type.GetCustomAttribute<TableAttribute>();
                if (tableAttribute != null)
                {
                    entityType.SetAnnotation(RelationalAnnotationNames.TableName, tableAttribute.Name);
                }
                var properties = new List<Property>();
                var primarkKeyProperties = new List<Property>();
                foreach (var propertyInfo in type.GetProperties())
                {

                    if (propertyInfo.GetCustomAttribute<NotMappedAttribute>() != null)
                    {
                        continue;
                    }
                    var property = entityType.AddProperty(propertyInfo);

                    // 如果数据生成选项是Identity或者没有配置数据生成选项，都认为它是自增列
                    if (propertyInfo.Name == "Id")
                    {
                        var databaseGenerated = propertyInfo.GetCustomAttribute<DatabaseGeneratedAttribute>();
                        if (databaseGenerated == null)
                        {
                            property.SetValueGenerated(ValueGenerated.OnAdd);
                        }
                        else if (databaseGenerated.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                        {
                            property.SetValueGenerated(ValueGenerated.OnAdd);
                        }
                    }

                    if (propertyInfo.Name == "Id" || propertyInfo.GetCustomAttribute<KeyAttribute>() != null)
                    {
                        primarkKeyProperties.Add(property);
                    }
                    var column = propertyInfo.GetCustomAttribute<ColumnAttribute>();
                    if (column != null)
                    {
                        property.SetAnnotation(RelationalAnnotationNames.ColumnName, column.Name);
                    }

                }
                if (primarkKeyProperties.Count > 0)
                {
                    entityType.SetPrimaryKey(new Key(primarkKeyProperties));
                }
                return entityType;
            });
        }

    }
}
