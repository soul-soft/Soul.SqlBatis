using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Soul.SqlBatis.Infrastructure
{
	public interface IEntityEntry: IEntityType
	{
		object Entity { get; }
		EntityState State { get; set; }
		IReadOnlyCollection<IEntityPropertyEntry> Values { get; }
	}

	public class EntityEntry : IEntityEntry
	{
		public object Entity { get; }

		private readonly IEntityType _entityType;

		private readonly IReadOnlyCollection<EntityPropertyEntry> _values;

		public EntityEntry(IEntityType entityType, object entity, IReadOnlyCollection<EntityPropertyEntry> valus)
		{
			Entity = entity;
			_values = valus;
			_entityType = entityType;
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
				else
				{
					_state = EntityState.Unchanged;
				}
				return _state;
			}
			set
			{
				if (value == EntityState.Modified)
				{
					foreach (var item in _values)
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

		public virtual IReadOnlyCollection<IEntityPropertyEntry> Values => _values;

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

	public class EntityEntry<T> : IEntityEntry
	{
		private readonly IEntityEntry _entry;

		public T Entity => (T)_entry.Entity;

		object IEntityEntry.Entity => _entry.Entity;

		public EntityState State { get => _entry.State; set => _entry.State = value; }

		public IReadOnlyCollection<IEntityPropertyEntry> Values => _entry.Values;

		public Type Type => _entry.Type;

		public string Schema => _entry.Schema;

		public string TableName => _entry.TableName;

		public IReadOnlyCollection<object> Metadata => _entry.Metadata;

		public IReadOnlyCollection<IEntityPropertyType> Properties => _entry.Properties;

		internal EntityEntry(IEntityEntry entry)
		{
			_entry = entry;
		}

		public IEntityPropertyType GetProperty(MemberInfo member)
		{
			return _entry.GetProperty(member);
		}

		public void HasAnnotation(object annotation)
		{
			_entry.HasAnnotation(annotation);
		}
	}

	internal class EntityEntryCache : IEntityEntry
	{
		private IEntityType _entityType;

		public IReadOnlyCollection<IEntityPropertyEntry> Values { get; }

		public object Entity { get; }

		public EntityState State { get; set; }

		public EntityEntryCache(IEntityType entityType,object entity,IReadOnlyCollection<IEntityPropertyEntry> values, EntityState state)
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
