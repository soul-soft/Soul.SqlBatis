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

var gs = new List<Gender?>();
gs.Add(Gender.男);
gs.Add(Gender.女);
var pam = new DynamicParameters();
pam.Add("@GS", gs);
var entity = context.Set<Student>(pam)
    .Where("gender IN @GS")
    .AsTracking()
    .ToList();
Console.WriteLine();
