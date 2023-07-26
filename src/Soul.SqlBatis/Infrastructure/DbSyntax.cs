namespace Soul.SqlBatis
{
    public class DbSyntax
    {
        public string Raw { get; }

        public DbSyntax(string raw)
        {
            Raw = raw;
        }

        public static explicit operator string(DbSyntax syntax) => syntax.Raw;

        public static implicit operator DbSyntax(string raw) => new DbSyntax(raw);
    }
}
