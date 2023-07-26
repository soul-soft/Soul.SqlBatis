using Soul.SqlBatis;
using Soul.SqlBatis.Test;

var context = new MyDbContext();
var list = new int[] { };
var sql = context.Set<Student>()
	.Where("aa")
	.Select(a => a.Id)
	.ToList();
