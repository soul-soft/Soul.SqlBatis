using MySqlConnector;
using Soul.SqlBatis;
using Soul.SqlBatis.Test;

var context = new MyDbContext(new DbContextOptions
{
	ConnecionProvider = () => new MySqlConnection("")
});
var list = new int[] { };
var req = new { Age = 50 };
var names = new string[]
{
	"78",
	"74"
};
var sql = context.Students
	.FromSql("students")
	.Where(a => DbOperations.StartsWith(a.Name,"fa"))
	.ToList();
Console.WriteLine();
