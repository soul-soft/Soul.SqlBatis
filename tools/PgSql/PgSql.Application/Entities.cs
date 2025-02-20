

using System;
using Soul.SqlBatis;
using System.Text.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Entities
{	

	/// <summary>
    /// 
    /// </summary>
	[Table("student_scores", Schema = "public")]
	public partial class StudentScores : Entity
	{
		/// <summary>
		/// 
		/// </summary>
		[Key][Identity]
		[Column("Id")]
		[Description("")]
		public override int? Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		[Column("chinese")]
		[Description("")]
		public decimal? Chinese { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		[Column("stu_id")]
		[Description("")]
		public int? StuId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		[Column("math")]
		[Description("")]
		public decimal? Math { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		[Column("english")]
		[Description("")]
		public decimal? English { get; set; }
	}

	/// <summary>
    /// 学生信息表
    /// </summary>
	[Table("students", Schema = "public")]
	public partial class Students : Entity
	{
		/// <summary>
		/// 
		/// </summary>
		[Key][Identity]
		[Column("id")]
		[Description("")]
		public override int? Id { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		
		[Column("name")]
		[Description("名称")]
		public string? Name { get; set; }
		/// <summary>
		/// 年龄
		/// </summary>
		
		[Column("age")]
		[Description("年龄")]
		public int? Age { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		[Column("dep_ids")]
		[Description("")]
		public int[]? DepIds { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		[Column("dep_names")]
		[Description("")]
		public string[]? DepNames { get; set; }
	}

	/// <summary>
    /// 
    /// </summary>
	[Table("test", Schema = "public")]
	public partial class Test : Entity<string?>
	{
		/// <summary>
		/// 
		/// </summary>
		[Key]
		[Column("id")]
		[Description("")]
		public override string? Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		[Column("flag")]
		[Description("")]
		public bool? Flag { get; set; }
	}
}

