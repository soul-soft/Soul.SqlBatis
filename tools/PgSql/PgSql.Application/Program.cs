using Soul.SqlBatis;
using Soul.SqlBatis.Entities;
using System.Text.Json;

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
                configure.UseEntityMapper(configureOptions => 
                {
                    configureOptions.UseTypeMapper((dr, i) => 
                    {
                        var json = dr.GetString(i);
                        return JsonDocument.Parse(json).RootElement;
                    });
                });
                configure.UseNpgsql(new Npgsql.NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=1024;Database=postgres;SSL Mode=Disable;"));
            });
            var sb1 = context.CreateSqlBuilder();
            var stu = new Students()
            {
                Js = JsonDocument.Parse("{\"id\":1}").RootElement
            };
            context.Add(stu);
            context.SaveChanges();
            context.SaveChanges();
        }
    }
}
