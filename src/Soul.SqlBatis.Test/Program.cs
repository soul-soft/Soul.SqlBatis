using Npgsql;
using Soul.SqlBatis;
using Soul.SqlBatis.Test;
using Soul.SqlBatis.Test.Entities;


Fata.F();
var context = new DbContext(configureOptions =>
{
    configureOptions.UseLogger((sql, param) =>
    {
        Console.WriteLine(sql);
    });
    configureOptions.UseNpgsql(new NpgsqlConnection("Host=127.0.0.1;Port=5432;Username=postgres;Password=1024;Database=postgres;SSL Mode=Disable;"));
});
var list1 = context.Set<Student>().ToList();
var list2 = context.Set<Student>().ToList();
Console.WriteLine();
