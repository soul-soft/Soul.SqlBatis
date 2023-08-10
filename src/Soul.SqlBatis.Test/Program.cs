using Microsoft.Extensions.Logging;
using MySqlConnector;
using Soul.SqlBatis;
using Soul.SqlBatis.Test;

var context = new MyDbContext(new DbContextOptions
{
	LoggerFactory = LoggerFactory.Create(logging =>
	{
		logging.AddConsole();
	}),
	ConnecionProvider = () => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test")
});

var student = context.Students
	.Where("Id IN (SELECT StudentId FROM student_scores WHERE math > @Math)", new { Math = 90 })
	.Where(a => DbOperations.Contains(a.FirstName, "王"))
	.ToList();




Console.WriteLine();