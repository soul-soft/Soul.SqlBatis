using MySqlConnector;
using Npgsql;
using Soul.SqlBatis;
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
    configureOptions.UseNpgsql(new MySqlConnection("server=127.0.0.1;user id=root;password=1024;database=test;pooling=True;minpoolsize=5;maxpoolsize=200;characterSet=utf8;Allow User Variables=true"));
}))
{
    var p = new DynamicParameters();
    p.Add("@P1", new int[] { 1, 2});
    p.Add(" @P2", new int[] { 1, 2 });
    var list = context.Sql.Query<Student>("select * from students where category_id IN @P1 AND category_sec_id IN @P2", p);


}
