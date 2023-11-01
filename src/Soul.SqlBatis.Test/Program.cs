using MySql.Data.MySqlClient;
using Soul.SqlBatis;
using Soul.SqlBatis.Entities;
using Soul.SqlBatis.Infrastructure;

var options = new DbContextOptionsBuilder()
	.EnableQueryTracking()
	.UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"))
	.Build();
var context = new MyDbContext(options);

try
{
	//var student = new Student();
	var student = new Student()
	{
		Id = 20,
		Name = "Test",
		Rowversion = "4003cb53-8f07-4dda-ace1-184830e9b20b"
    };
    context.Attach(student);
    var state1 = context.Entry(student).State;
	student.Rowversion = "1";
    var state2 = context.Entry(student).State;
    context.SaveChanges();
}
catch (Exception ex)
{

	throw;
}

