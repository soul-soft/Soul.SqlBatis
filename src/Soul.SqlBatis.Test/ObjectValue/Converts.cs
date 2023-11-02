namespace Soul.SqlBatis
{
    internal static class Converts
    {
        public static string DatetimtToString(DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }
    }
}
