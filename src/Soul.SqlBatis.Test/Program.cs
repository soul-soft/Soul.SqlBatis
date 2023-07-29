using Soul.SqlBatis;
using Soul.SqlBatis.Test;

var context = new MyDbContext();
var list = new int[] { };
var req = new { Age = 50 };
var sql = context.Set<Student>()
    .Where("age = 20")
    .Where(a => DbFunc.Raw<int>("COUNT(*)") > req.Age)
    .ToList();
