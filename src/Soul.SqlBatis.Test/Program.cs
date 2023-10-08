using MySql.Data.MySqlClient;
using Soul.SqlBatis;
using Soul.SqlBatis.Infrastructure;

var options = new DbContextOptionsBuilder()
	.EnableQueryTracking()
	.UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"))
	.Build();
var context = new MyDbContext(options);

try
{
    var param1 = new DynamicParameters();
    var sb1 = await context.Students
        .ToListAsync();
}
catch (Exception ex)
{

	throw;
}

