namespace Soul.SqlBatis
{
    public class SqlToken
    {
        public string Raw { get; }

        public SqlToken(string raw)
        {
            Raw = raw;
        }

        public static explicit operator string(SqlToken syntax) => syntax.Raw;

        public static implicit operator SqlToken(string raw) => new SqlToken(raw);
    }
}
