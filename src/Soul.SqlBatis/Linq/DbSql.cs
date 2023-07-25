namespace Soul.SqlBatis
{
    public class DbSql
    {
        public string CommandText { get; }

        public DbSql(string commandText)
        {
            CommandText = commandText;
        }

        public static explicit operator string(DbSql syntax) => syntax.CommandText;
        public static implicit operator DbSql(string commandText) => new DbSql(commandText);
    }
}
