using Npgsql;
using Soul.SqlBatis;
using Soul.SqlBatis.Test;
using Soul.SqlBatis.Test.Entities;


DbContextSettings.Configure(DbType.Npgsql, configure =>
{
    configure.UseDbNullMapper(-1);
    configure.UseDbNullMapper(Array.Empty<int>());
    configure.UseTypeMapper((record, i) =>
    {
        return ((NpgsqlDataReader)record).GetFieldValue<int[]>(i);
    });
});
var context = new DbContext(configureOptions =>
{
    configureOptions.UseLogger((sql, param) =>
    {
        Console.WriteLine(sql);
    });
    configureOptions.UseNpgsql(new NpgsqlConnection("Host=127.0.0.1;Port=5432;Username=postgres;Password=1024;Database=postgres;SSL Mode=Disable;"));
});
//var list1 = context.Set<Student>().OrderByDescending(a => a.Id).Select(s => s.DepIds).ToList();
//var list2 = context.Set<Student>().Select(s => s.DepIds).ToPageResult(1, 10);
//var list3 = context.Set<Student>().GroupBy(a => a.Age).Select(s => new { s.Age, Count = DbFunc.Count() }).ToList();

var entity = context.Set<Student>().Where(a => a.Id == 2).AsTracking().Single();
entity.Name = "gaga";
context.SaveChanges();
Console.WriteLine();
