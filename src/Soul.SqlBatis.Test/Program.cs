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
    var student = context.Students.Where(a=>a.Id == 1).First();
	var state1 = context.Entry(student).State;
	student.Name = "fa";
    var state2 = context.Entry(student).State;
	context.Update(student);
    context.SaveChanges();
}
catch (Exception ex)
{

    throw;
}

