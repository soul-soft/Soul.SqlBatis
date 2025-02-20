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
                    configureOptions.UseTypeMapper((dr, i) =>
                    {
                        return (dr as Npgsql.NpgsqlDataReader)!.GetFieldValue<int[]>(i);
                    });
                    configureOptions.UseTypeMapper((dr, i) =>
                    {
                        return (dr as Npgsql.NpgsqlDataReader)!.GetFieldValue<string[]>(i);
                    });
                });
                configure.UseNpgsql(new Npgsql.NpgsqlConnection("Host=localhost;Port=5432;Username=postgres;Password=1024;Database=postgres;SSL Mode=Disable;"));
            });
            var stu = new Students()
            {
                Age = 1,
                DepIds = new int[] { 1, 2, 3 },
                DepNames = new string[] { "wjf", "zyy" },
                Name = "wjf"
            };
            context.Add(stu);
            context.SaveChanges();
            var list = context.Set<Students>().ToList();
            context.SaveChanges();
        }
    }
}
