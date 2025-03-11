using Npgsql;
using Soul.SqlBatis;
using Soul.SqlBatis.Test.Entities;
DbContextSettings.Configure(DbType.MySql, configure =>
{
});
var context = new DbContext(configureOptions =>
{
    configureOptions.UseNpgsql(new NpgsqlConnection("Host=127.0.0.1;Port=5432;Username=postgres;Password=1024;Database=postgres;SSL Mode=Disable;"));
});
var list1 = context.Set<Student>().Select(s => s.Id).ToList();
var list2 = context.Set<Student>().Select(s => s.Age).ToList();
Console.WriteLine();
