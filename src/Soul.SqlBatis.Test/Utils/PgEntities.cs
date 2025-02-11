

using System;
using Soul.SqlBatis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Entities
{	

	/// <summary>
    /// 
    /// </summary>
	[Table("student_scores")]
	public partial class student_scores : Entity
	{
		/// <summary>
		/// 
		/// </summary>
		public  string Math { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public  string Stuid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public  string English { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public override string Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public  string Chinese { get; set; }
	}

	/// <summary>
    /// 学生信息表
    /// </summary>
	[Table("students")]
	public partial class students : Entity
	{
		/// <summary>
		/// 
		/// </summary>
		public override string Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public  string Age { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public  string Name { get; set; }
	}

	/// <summary>
    /// 
    /// </summary>
	[Table("v_students")]
	public partial class v_students : Entity
	{
		/// <summary>
		/// 
		/// </summary>
		public  string English { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public  string Name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public  string Chinese { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public override string Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public  string Age { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public  string Math { get; set; }
	}
}

