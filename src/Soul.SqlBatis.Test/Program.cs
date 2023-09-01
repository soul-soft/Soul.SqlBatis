using Microsoft.Extensions.Logging;
using Soul.SqlBatis;
using Soul.SqlBatis.Infrastructure;
DbContextOptionsBuilder.AddConnectionFactory(ConnectionProvider.MySql, connectionString =>
{
    return new MySql.Data.MySqlClient.MySqlConnection(connectionString);
});
var options = new DbContextOptionsBuilder()
    .EnableTracking()
    .UseMySql("Server=127.0.0.1;Port=3306;User ID=root;Password=1024;Database=test")
    .UseLoggerFactory(LoggerFactory.Create(configure =>
    {
        configure.AddConsole();
    }))
    .Build();

var context = new MyDbContext(options);
var context1 = new MyDbContext(options);
var list = new List<int>()
{

};
var students = context.Students
    .Where(a => a.CreationTime > DateTime.Now)
    .ToList();
var row = await context.SaveChangesAsync();
Console.WriteLine();