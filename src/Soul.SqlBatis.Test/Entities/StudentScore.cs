using System;
using Soul.SqlBatis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Entities
{	
	/// <summary>
    /// 
    /// </summary>
	[Table("student_score")]
	public partial class StudentByScore
	{
		/// <summary>
		/// 
		/// </summary>
		[Column("Chinese")]
		public double? Chinese { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Key][Column("id")]
		public int Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Math")]
		public double? Math { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("StuId")]
		public int? Stuid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Title")]
		public string Title { get; set; }
	}
}
