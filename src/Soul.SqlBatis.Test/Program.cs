using Soul.SqlBatis;
using Soul.SqlBatis.Test;

var context = new MyDbContext();
var list = new int[] { };
var req = new { Age = 50 };
var sql = context.Students
	.FromSql("students")
	.Where(a => a.Id > 0)
	.ToList();
Console.WriteLine();
