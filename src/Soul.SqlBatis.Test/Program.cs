using MySql.Data.MySqlClient;
using Soul.SqlBatis;
using Soul.SqlBatis.Entities;
using Soul.SqlBatis.Infrastructure;

var options = new DbContextOptionsBuilder()
	.AsTracking()
	.UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"))
	.Build();
var context = new MyDbContext(options);


var param1 = new DynamicParameters();
var sb1 = context.Students
	.Where(a=>DbOperations.In(a.Id,1,2,3))
	.ToList();
