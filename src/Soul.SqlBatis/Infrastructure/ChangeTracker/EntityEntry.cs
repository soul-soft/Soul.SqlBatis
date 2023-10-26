using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public class EntityEntry : IEntityType
	{
		public object Entity { get; }

		internal IEntityType EntityType { get; }

		public EntityEntry(IEntityType entityType, object entity, IReadOnlyCollection<EntityPropertyEntry> valus)
		{
			Entity = entity;
			Values = valus;
			EntityType = entityType;
		}

		private EntityState _state;

		public virtual EntityState State
		{
			get
			{
				if (Values.Any(a => a.IsModified))
				{
					_state = EntityState.Modified;
				}
				return _state;
			}
			set
			{
				if (value == EntityState.Modified)
				{
					foreach (var item in Values)
					{
						if (item.IsKey)
						{
							continue;
						}
						if (item.IsConcurrencyToken)
						{
							continue;
						}
						item.OriginalValue = null;
					}
				}
				_state = value;
			}
		}

		public virtual IReadOnlyCollection<EntityPropertyEntry> Values { get; }

		public Type Type => EntityType.Type;

		public string Schema => EntityType.Schema;

		public string TableName => EntityType.TableName;

		public IReadOnlyCollection<object> Metadata => EntityType.Metadata;

		public IReadOnlyCollection<IEntityPropertyType> Properties => EntityType.Properties;

		public IEntityPropertyType GetProperty(MemberInfo member)
		{
			return EntityType.GetProperty(member);
		}

		public void HasAnnotation(object annotation)
		{
			EntityType.HasAnnotation(annotation);
		}
	}

	public class EntityEntry<T> : EntityEntry
	{
		private readonly EntityEntry _entry;

		public new T Entity => (T)_entry.Entity;

		public override EntityState State { get => _entry.State; set => _entry.State = value; }

		public override IReadOnlyCollection<EntityPropertyEntry> Values => _entry.Values;

		internal EntityEntry(EntityEntry entry)
			: base(entry.EntityType, entry.Entity, entry.Values)
		{
			_entry = entry;
		}
	}
}
