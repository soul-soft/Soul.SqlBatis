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
                configure.UseQueryTracking();
                configure.UseLogger((sql, param) =>
                {
                    Console.WriteLine(sql);
                });
                configure.UseNpgsql(new Npgsql.NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=1024;Database=postgres;SSL Mode=Disable;"));
            });
            var sb1 = context.CreateSqlBuilder();
            var ids = new int?[] { };
            var list = context.Set<Students>()
                .Where(a => ids.Contains(a.Id))
                .ToPageResult(1, 5);
            context.SaveChanges();
        }
    }
}
