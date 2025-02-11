using Soul.SqlBatis;
using Soul.SqlBatis.Entities;

namespace PgSql.Application
{
    public class Program
    {
        public static void Main()
        {
            var context = new DbContext(configure =>
            {
                configure.UsePgSql(new Npgsql.NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=1024;Database=postgres;SSL Mode=Disable;"));
            });
            var student = new Students()
            {
                Name = "zs"
            };
            var list = context.Set<Students>().ToPageResult(2, 5);
            context.SaveChanges();
        }
    }
}
