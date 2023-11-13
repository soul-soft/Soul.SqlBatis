using Soul.SqlBatis;
using Soul.SqlBatis.Infrastructure;

var options = new DbContextOptionsBuilder()
    //.EnableQueryTracking()
    //.UseConnectionFactory(() => new MySqlConnector.MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test") ,DBMS.MYSQL)
    .UseConnectionFactory(() => new Microsoft.Data.SqlClient.SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"), DBMS.MSSQL)
    .Build();

TypeMapper.AddTypeMapper<DateTime, string>(date =>
{
    return date.ToString("yyyy/MM/dd");
});

using var context = new MyDbContext(options);
var list = await context.Students.ToListAsync();
context.SaveChanges();
Console.WriteLine();
