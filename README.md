# Soul.SqlBatis
这款ORM设计思想以查询性能为主极低的配置代价，支持动态实体配置、实体变更跟踪、动态sql查询、Linq查询、sql+Linq混合查询。

## DbContext配置


``` C#
public partial class MyDbContext : DbContext
{
    readonly ILogger _logger;

    public MyDbContext()       
    {
        
    }

    public MyDbContext(DbContextOptions options)
        :base(options)
    {
        
    }

    public MyDbContext(DbContextOptions options, ILogger<MyDbContext> logger)
        :base(options)
    {
        _logger = logger;
    }	

    protected override void Logging(string sql, object param)
    {
	_logger?.LogInformation(sql);	
    }

	
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
	//只有无参构造的DbContext才通过OnConfiguring来配置
        optionsBuilder.UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"));
    }
}

var options = new DbContextOptionsBuilder()
	.EnableQueryTracking()//默认启用实体更改跟踪
	.UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"))
	.Build();
var context = new MyDbContext(options);
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

调用数据库上下文的Add、Update、Delete操作实体更加安全，而且都是在内存中完成，如果启用了查询更改跟踪，对所有的值类型的更改，都会监听到。执行SaveChange时检测实体状态，如果是Add、或者Delete那么直接新增或者删除对象。如果是Update，则只更新更改的属性。
注意：由于更改跟踪是基于内存地址比较的，对于json类型的字段，不能只更改json实体的属性，而且创建一个新的对象（ObjectValue设计原则）。

- 主动告知

  ```C#
    var student = new Student()
    {
        Name="SqlBatis"
    };
    context.Entry(student).State = EntityState.Added;
    //Or
    context.Add(student);
    //提交成功之后，清除当前上下文跟踪的所有实体的引用
    context.SaveChanges();
  ```

- 查询转换

  ```C#
    //如果context中存在，则直接返回，否则查询数据库，并加入跟踪
    var studentCache = context.Find<Student>(1);
   
   //只跟踪不读缓存
    var student = context.Students
        .Where(a => a.Id == 1)
	//主动跟踪
  	.AsTracking()
        //不进行跟踪
  	.AsNoTracking()
        .First();
    student.Name = "zs";
    context.SaveChanges();		
  ```

- 实体状态

  ```c#
    //状态被标记为UnChanged，并且永不改变，此时SqlBatis会和原始值进行比对，如果字段被修改则只更新修改的字段
    var student1 = context.Students
    .Where(a => a.Id == 1)
    .AsTracking()
    .First();
    student1.Name = "cw";
    var student2 = new Student
    {
        Name = "zs"    
    };
    //实体被标记为Added，保存之后返回自增id
    context.Add(student2);
    var student3 = new Student
    {
        Id = 2,
        Name = "zs"    
    };
    //实体被标记为Modified，由于数据上下文没有对student3进行跟踪，无法知道它的原始值，无法判断哪些字段被修改了，此时将执行全量字段更新（考虑到查询一次对数据一样有压力）
    context.Update(student3);
    //实体被标记为Deleted
    context.Delete(new 
    {
        Id = 3    
    });
    //查询跟踪状态    
    var entry = context.Entry(student3);
  ```

- json

  1. json类型必须通过JsonValue注解类型，对于json数组，框架内置了一个JsonArray，并实现了更改跟踪。

  2. 由于SqlBatis是基于内存地址进行更改跟踪的，对于引用类型将失效。json对象建议使用结构体或者record（这符合值对象设计原则）或者你可以实现INotifyPropertyChanged接口

  ```C#
    //必须遵循值类型原则
    [JsonValue]
    public record Address
    {
        public string City { get; set; }
    }
  ```

  

## 连接和事务

- 如果DbContext存在事务（CurrentTransaction）调用SaveChanges，使用当前事务（CurrentTransaction）。如果DbContext不存在事务，则自动开启一个事务，自动提交。如果提交成功，清除当前DbContext实列跟踪的实体，并释放关闭事务。
- 开启事务之前会自动判断是否开启数据库连接，如果未开启则自动开启提交或者回滚自动关闭。
- 查询数据之前会判断当前连接是否开启，如果未开启则自动开启，查询完毕之后立即关闭。如果一个上下文实列涉及频繁查询，则将导致频繁开关数据库连接。建议使用AOP技术进行代理，在业务方法开始手动打开连接，业务方法结束手动关闭。

## 查询数据

- 基本查询

  ```C#
    var student = context.Students.Where(a=>a.Id == 20).First();
    var students = context.Students.Skip(10).Take(10).ToList(); 
  ```

- Sql To Linq

  ```C#
  var sb = new SqlBuilder();
  //model是请求模型
  var param = new DynamicParameters();
  param.Add(model);//将model进行解构
  sb.Where("schools.Math > @MathMin " ,  model.MathMin != null);
  var whereSql = sb.Build("/**where**/");
  //告别视图
  var view = $@"
  SELECT 
  	SchoolName,
  	FirstName,
  	COUNT(*) Count,
  FROM 
  	students join schools ON students.SchoolId = schools.Id
  {whereSql}
  GROUP BY
  	SchoolName,
  	FirstName
  ";
  var student = context.FromSql<StudentGroup>(view, param)
  	.Where(a => DbOperations.Contains(a.FirstName, "王"))
    .Take(10).Skip(0)//分页  
  	.ToList();
  ```
- Linq To Sql 

  ``` C#
	var (sb,param) = context.Students
		.Where(a => a.Id  > 10)
		.Where(a => a.Math > 20)
		.Build();
	var whereSql = sb.Build("/**where**/");
	var sql = $"select * from student {whereSql}"
	var list = context.Query(sql, param);
	```
	- 混合查询
	
	  ```C#
	  //尽情发挥创造力
	  var param = new DynamicParameters();
	  param.Add(new { Math = 90 });
	  var student = context.Set<Student>(param)
	  	.Where("Id IN (SELECT StudentId FROM student_scores WHERE math > @Math)")
	  	.Where(a => DbOperations.Contains(a.FirstName, "王"))
	  	.ToList();
  ```
- switch、if

  ``` C#
   var list = context.Students
        .Select(s => new
        {
            Fa = s.State,
            Flag = DbOperations.IF(s.State > 0, "大于0", "小于0"),
            Status = DbOperations.Switch(s.State == 0, "初始").Case(s.State == 1, "VIP").Default("游离")
        })
        .ToList();
  ```  

## 函数映射

编写一个静态类，给成员函数加上DbFunction即可，可以通过Name来映射名称，通过Format来映射参数

```C#
public static class DbFunctions
{
	[DbFunction(Name = "NOW")]
	public static DateTime Now()
	{
	    throw new NotImplementedException();
	}
	
	[DbFunction(Name = "COUNT", Format = "DISTINCT {column}")]
	public static int CountDistinct(object column)
	{
	    throw new NotImplementedException();
	}
	
	[DbFunction(Name = "COUNT", Format = "*")]
	public static int CountX()
	{
	    throw new NotImplementedException();
	}
	
	[DbFunction(Name = "AVG")]
	public static decimal Avg<T>(T column)
	{
	    throw new NotImplementedException();
	}
	
	[DbFunction(Name = "JSON_EXTRACT")]
	public static TPath JsonExtract<TPath>(object column, string path)
	{
	    throw new NotImplementedException();
	}
	
	[DbFunction(Name = "DATE")]
	internal static DateTime Date(DateTime? creationTime)
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
        Count = DbFunctions.Count(s.Id)        
    });
```

