namespace Soul.SqlBatis
{
    public class DbSqlExpression
    {
        public string Raw { get; }

        public DbSqlExpression(string raw)
        {
            Raw = raw;
        }

        public static explicit operator string(DbSqlExpression syntax) => syntax.Raw;

        public static implicit operator DbSqlExpression(string raw) => new DbSqlExpression(raw);
    }
}
