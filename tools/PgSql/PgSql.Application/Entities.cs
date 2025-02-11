

using System;
using Soul.SqlBatis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

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
		public override int? Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		public decimal? Chinese { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		public int? StuId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		public decimal? Math { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
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
		public override int? Id { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		
		public string? Name { get; set; }
		/// <summary>
		/// 年龄
		/// </summary>
		
		public int? Age { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		public decimal? M { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		public float? F { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		public JsonElement? Js { get; set; }
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
		public override string? Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		
		public bool? Flag { get; set; }
	}
}

