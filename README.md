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
    .OrderBy("students.age desc")
    .Page(1, 20);

var querySql = sb.Select("students.id,students.age,schools.name");
var countSql = sb.Count();
```

## 模板方式

``` C#
var sb = new SqlBuilder();
sb.Where("students.age > @Age", req.Age != null);
sb.Where("students.name LIKE @Name", req.Name != null);
sb.OrderBy("students.age desc");
var countSql = sb.Build("SELECT COUNT(*) FROM /**WHERE**/");
var querySql = sb.Build("SELECT * FROM /**WHERE**/ /**ORDERBY**/");
```

## 配合dapper

``` C#
var req = new
{
    Age = (int?)50
};
var values = new DynamicValues(req);
var connection = new MysqlConnection("...");
var sb = new SqlBuilder();
sb = sb.From("students join schools on students.sch_id = schools.id")
    .Where("students.age > @Age", req.Age != null)
    .OrderBy("students.age desc")
    .Page(1, 20);

var querySql = sb.Select("students.id,students.age,schools.name");
var countSql = sb.Count();
var list = connection.Query(querySql, values);
var count = connection.ExecuteScalar<int>(countSql, values);
```
