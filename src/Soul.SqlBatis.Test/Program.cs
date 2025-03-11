using Npgsql;
using Soul.SqlBatis;
using Soul.SqlBatis.Test.Entities;
DbContext.Settings.Npgsql.UseTypeMapper((record, i) =>
{
    return record.GetInt32(i);
});
DbContext.Settings.Npgsql.UseTypeMapper((record, i) =>
{
    return record.GetString(i);
});
var context = new DbContext(configureOptions =>
{
    configureOptions.UseNpgsql(new NpgsqlConnection("Host=127.0.0.1;Port=5432;Username=postgres;Password=1024;Database=postgres;SSL Mode=Disable;"));
});
var list = context.Set<Student>().Select(s => s.Id).ToList();
Console.WriteLine();
