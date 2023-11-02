using Microsoft.Data.SqlClient;
using Soul.SqlBatis;
using Soul.SqlBatis.Entities;
using Soul.SqlBatis.Infrastructure;

var options = new DbContextOptionsBuilder()
    //.EnableQueryTracking()
    //.UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"))
    .UseConnectionFactory(() => new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"))
    .Build();

using var context = new MyDbContext(options);
context.OpenDbConnection();
var list1 = context.Query<StudentDto>("select money from student").ToList();
foreach (var item in list1)
{
    Console.WriteLine(item);
}
var list2 = context.Query<decimal>("select money from student");
int w = 2;
decimal a = (decimal)w;
Console.WriteLine();


Console.WriteLine(  );
