using System;
using System.Collections.Generic;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	internal class EntityEntryCache : IEntityEntry
	{
		private IEntityType _entityType;

		public IReadOnlyCollection<IEntityPropertyEntry> Values { get; }

		public object Entity { get; }

		public EntityState State { get; set; }

		public EntityEntryCache(IEntityType entityType, object entity, IReadOnlyCollection<IEntityPropertyEntry> values, EntityState state)
		{
			_entityType = entityType;
			Entity = entity;
			Values = values;
			State = state;
		}

		public Type Type => _entityType.Type;

		public string Schema => _entityType.Schema;

		public string TableName => _entityType.TableName;

		public IReadOnlyCollection<object> Metadata => _entityType.Metadata;

		public IReadOnlyCollection<IEntityPropertyType> Properties => _entityType.Properties;

		public IEntityPropertyType GetProperty(MemberInfo member)
		{
			return _entityType.GetProperty(member);
		}

		public void HasAnnotation(object annotation)
		{
			_entityType.HasAnnotation(annotation);
		}
	}
}
