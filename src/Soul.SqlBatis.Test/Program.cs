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
	var student = await context.Students.FirstAsync();
	student.Firstname = "zz122";
	var list = context.Staffs.ToList();

    context.SaveChanges();
}
catch (Exception ex)
{

	throw;
}

