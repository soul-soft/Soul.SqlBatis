using Soul.SqlBatis;
using Soul.SqlBatis.Infrastructure;

var options = new DbContextOptionsBuilder()
    //.EnableQueryTracking()
    .UseConnectionFactory(() => new MySqlConnector.MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"))
    //.UseConnectionFactory(() => new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"))
    .Build();

using var context = new MyDbContext(options);
context.OpenDbConnection();

var list1 = context.Students.ToList();
var list2 = context.Students.ToList();
int w = 2;
decimal a = (decimal)w;
Console.WriteLine();


Console.WriteLine(  );
