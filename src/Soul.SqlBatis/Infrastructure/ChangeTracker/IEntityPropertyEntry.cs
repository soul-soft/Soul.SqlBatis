namespace Soul.SqlBatis.Infrastructure
{
	public interface IEntityPropertyEntry : IEntityPropertyType
	{
		object CurrentValue { get; }
		object OriginalValue { get; }
		bool IsModified { get; }
	}
}
