namespace Soul.SqlBatis
{
    public class DbQueryableOptions
    {
        public bool UseDefaultOrder { get; set; } = false;
        public bool HasColumnsAlias { get; set; } = true;
        public bool HasDefaultColumns { get; set; } = true;
    }
}
