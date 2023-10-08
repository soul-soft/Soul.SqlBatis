using System;
using Soul.SqlBatis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Entities
{	
	/// <summary>
    /// 
    /// </summary>
	[Table("staff")]
	public partial class Staff
	{
		/// <summary>
		/// 
		/// </summary>
		[Column("Company")]
		public string Company { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Grade")]
		public string Grade { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Key][Column("Id")]
		public int Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Name")]
		public string Name { get; set; }
	}
}
