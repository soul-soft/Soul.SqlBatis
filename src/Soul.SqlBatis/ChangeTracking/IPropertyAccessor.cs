namespace Soul.SqlBatis.ChangeTracking
{
    public interface IPropertyAccessor
    {
        object GetPropertyValue(string name);
        void SetPropertyValue(string name, object value);
    }
}
