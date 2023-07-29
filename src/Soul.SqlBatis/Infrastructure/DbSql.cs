namespace Soul.SqlBatis
{
    public class DbSql
    {
        public string Raw { get; }

        public DbSql(string raw)
        {
            Raw = raw;
        }

        public static explicit operator string(DbSql syntax) => syntax.Raw;

        public static implicit operator DbSql(string raw) => new DbSql(raw);
    }
}
