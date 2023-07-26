using Soul.SqlBatis;
using Soul.SqlBatis.Test;

var context = new MyDbContext();
var list = new int[] { };
var req = new { Age = 50 };
var sql = context.Set<Student>()
    .Where("age = 20")
    .Where(a => DbFunc.Raw<int>("COUNT(*)") > req.Age)
    .Select(a => new 
    {
        Balance = DbFunc.Raw<int>("SUM(Banalce) OVER (PARTITION by NAME)")
    })
    .ToList();
