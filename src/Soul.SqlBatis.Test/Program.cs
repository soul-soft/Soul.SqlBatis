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


using (var context = new DbContext(configureOptions =>
{
    configureOptions.UseLogger((sql, param) =>
    {
        Console.WriteLine(sql);
    });
    configureOptions.UseNpgsql(new NpgsqlConnection("Host=127.0.0.1;Port=5432;Username=postgres;Password=1024;Database=postgres;SSL Mode=Disable;"));
}))
{
    var entity = context.Set<Student>()
        .ToPageResult(1,10);
}



