namespace Soul.SqlBatis
{
    public class DbSqlToken
    {
        public string Raw { get; }

        public DbSqlToken(string raw)
        {
            Raw = raw;
        }

        public static explicit operator string(DbSqlToken syntax) => syntax.Raw;

        public static implicit operator DbSqlToken(string raw) => new DbSqlToken(raw);
    }
}
