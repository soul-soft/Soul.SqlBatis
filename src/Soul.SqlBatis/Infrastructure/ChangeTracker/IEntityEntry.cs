using System.Collections.Generic;

namespace Soul.SqlBatis.Infrastructure
{
	public interface IEntityEntry : IEntityType
	{
		object Entity { get; }
		EntityState State { get; set; }
		IReadOnlyCollection<IEntityPropertyEntry> Values { get; }
	}

	public interface IEntityEntry<T> : IEntityEntry
	{
		new T Entity { get; }
	}
}
