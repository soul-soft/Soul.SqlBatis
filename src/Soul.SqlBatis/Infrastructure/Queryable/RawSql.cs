namespace Soul.SqlBatis
{
    public class RawSql
    {
        public string Raw { get; }

        public RawSql(string raw)
        {
            Raw = raw;
        }

        public static explicit operator string(RawSql syntax) => syntax.Raw;

        public static implicit operator RawSql(string raw) => new RawSql(raw);

        public override string ToString()
        {
            return Raw;
        }
    }
}
