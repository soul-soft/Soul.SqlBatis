using System.Collections.Generic;

namespace Soul.SqlBatis.Infrastructure
{
	public abstract class EntityEntry
	{
		public object Entity { get; }

		public virtual EntityState State { get; set; }

		public virtual IReadOnlyCollection<PropertyEntry> Properties { get; }

		public EntityEntry(object entity, IReadOnlyCollection<PropertyEntry> properties)
		{
			Entity = entity;
			Properties = properties;
		}
	}

	internal class InternalEntityEntry : EntityEntry
	{
		public InternalEntityEntry(object entity, IReadOnlyCollection<PropertyEntry> properties)
			: base(entity, properties)
		{
		}
	}

	public class EntityEntry<T> : EntityEntry
	{
		private readonly EntityEntry _entry;

		public new T Entity => (T)_entry.Entity;

		public override EntityState State { get => _entry.State; set => _entry.State = value; }

		public override IReadOnlyCollection<PropertyEntry> Properties => _entry.Properties;

		public EntityEntry(EntityEntry entry)
			: base(entry.Entity, entry.Properties)
		{
			_entry = entry;
		}
	}
}
