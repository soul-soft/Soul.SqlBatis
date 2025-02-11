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
                configure.UseLogger((sql, param) => 
                {
                    Console.WriteLine(sql);
                });
                configure.UsePgSql(new Npgsql.NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=1024;Database=postgres;SSL Mode=Disable;"));
            });
            var student = new Students()
            {
                Name = "zs"
            };
            context.Add(student);
            context.SaveChanges();
            var list = context.Set<Students>().ToPageResult(1, 5);
            context.SaveChanges();
        }
    }
}
