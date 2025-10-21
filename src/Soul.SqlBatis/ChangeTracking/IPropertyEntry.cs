namespace Soul.SqlBatis.ChangeTracking
{

    public interface IPropertyEntry
    {
        object OriginalValue { get; }

        object CurrentValue { get; }
    }
}
