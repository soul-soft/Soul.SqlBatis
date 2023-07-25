using Soul.SqlBatis;
using Soul.SqlBatis.Expressions;
using Soul.SqlBatis.Test;

var context = new MyDbContext();
var list = new int[] { };
var sql = context.Set<Student>()
	.FromSql("select * from student")
	.Where(a => Db.IsNull(a.Name))
	.Where("aa")
	.Select(a => a.Id)
	.ToList();
