# Soul.SqlBatis
这款ORM设计思想以查询性能为主极低的配置代价，支持动态实体、实体变更跟踪、动态sql查询、Linq查询、sql+Linq混合查询。

## DbContext配置

```C#
var context = new MyDbContext(new DbContextOptions
{
    LoggerFactory = LoggerFactory.Create(logging =>
    {
        logging.AddConsole();
    }),
    ConnecionProvider = () => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test")
});
```

## 实体配置

配置支持多种混合方式，fluent api配置优先级最高。实体配置以DbContext类型为key进行缓存。默认认为Id是主键和自增标识。

- ### 动态配置

  开箱即用，如果实体的字段名和类型名称同表名，则无需任何配置，动态构建。并同时支持注解和fluent api配置。

- ### attribute

  ```C#
  [Table("students")]
  public class Student
  {
      [Key]
      [Identity]
      public uint Id { get; set; }
      
      public string Name { get; set; }
      [Column("first_name")]

      public string FirstName { get; set; }
  
      public DateTime CreationTime { get; set; }
  }
  ```

- ### fluent api

  ```C#
  public class MyDbContext : DbContext
  {
  	public MyDbContext(DbContextOptions options)
  		: base(options)
  	{
  
  	}
  
  	public DbSet<Student> Students => Set<Student>();
  
  	protected override void OnModelCreating(ModelBuilder builder)
  	{
  		//忽略字段
  		builder.Entity<Student>().Igonre(a=>a.FirstName);
  		//移除默认的自增规则
  		builder.Entity<Student>().Property(a=>a.Id).ValueGeneratedNever();
  		builder.Entity<Student>().HasKey(a => a.Id);
  		builder.Entity<Student>().Property(a => a.FirstName).HasColumnName("FirstName");
  	}
  }
  
  ```

## 更改跟踪

要支持实体跟踪，请重写Entity的Equal和HashCode为主键字段，默认以实体的引用地址作为跟踪标识。【常识】

- 主动告知

  ```C#
  var student = new Student()
  {
      Name="SqlBatis"
  };
  context.Entry(student).State = EntityState.Added;
  //Or
  context.Add(student);
  context.SaveChanges();
  ```

- 查询转换

  ```C#
  var student = context.Students
  	.Where(a => a.Id == 1)
  	.AsTracking()//数据量大慎用，底层采用反射
  	.First();
  student.Name = "zs";
  context.SaveChanges();
  ```

## 保存数据

- 如果DbContext存在事务（CurrentTransaction）调用SaveChanges，使用当前事务。如果DbContext不存在事务，则自动开启一个事务，自动提交。如果提交成功，清除当前DbContext实列跟踪的实体。


- 开启事务之前会自动判断是否开启数据库连接，如果未开启则自动开启提交或者回滚自动关闭。

## 查询数据

- 基本查询

  ```C#
  var student = context.Students.Where(a=>a.Id == 20).First();
  var students = context.Students.Skip(10).Take(10).ToList();
  ```

- fromSql

  ```C#
  //告别视图
  var view = @"
  SELECT 
  	SchoolName,
  	FirstName,
  	COUNT(*) Count,
  FROM 
  	students join schools ON students.SchoolId = schools.Id
  GROUP BY
  	SchoolName,
  	FirstName
  ";
  var student = context.FromSql<StudentGroup>(view)
  	.Where(a => DbOperations.Contains(a.FirstName, "王"))
  	.ToList();
  ```

- 混合查询

  ```C#
  //尽情发挥创造力
  var student = context.Students
  	.Where("Id IN (SELECT StudentId FROM student_scores WHERE math > @Math)", new { Math = 90 })
  	.Where(a => DbOperations.Contains(a.FirstName, "王"))
  	.ToList();
  ```

## 函数映射

编写一个静态类，给成员函数加上DbFunction即可

```C#
public static class DbFunctions
{
    [DbFunction(Name = "NOW")]
    public static DateTime Now()
    {
        throw new NotImplementedException();
    }

    [DbFunction(Name = "COUNT")]
    public static long Count<T>(T column)
    {
        throw new NotImplementedException();
    }

    [DbFunction(Name = "AVG")]
    public static decimal Avg<T>(T column)
    {
        throw new NotImplementedException();
    }
}
```

```C#
var students = context.Students
    .GroupBy(a => a.FirstName)
    .Select(s => new 
    {
        s.FirstName,
        DbFunctions.Count(s.Id)        
    });
```

