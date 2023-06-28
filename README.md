# Soul.SqlBatis
这是一个sql构建工具

## 基本构建

``` C#
var req = new
{
    Age = (int?)50
};
var sb = new SqlBuilder();
sb = sb.From("students join schools on students.sch_id = schools.id")
    .Where("students.age > @Age", req.Age != null)
    .OrderBy("students.age desc");

var querySql = sb.Page(1,20)
    .Select("students.id,students.age,schools.name");
var countSql = sb.Page(1, 20).Count();
```

## 配合dapper

``` C#
var values = new DynamicValues();
var connection = new MysqlConnection("...");
var sb = new SqlBuilder();
sb = sb.From("students join schools on students.sch_id = schools.id")
    .Where("students.age > @Age", req.Age != null)
    .OrderBy("students.age desc");

var querySql = sb.Page(1,20)
    .Select("students.id,students.age,schools.name");
var countSql = sb.Page(1, 20).Count();
var list = connection.Query(querySql, values);
var count = connection.ExecuteScalar<int>(countSql, values);
```
