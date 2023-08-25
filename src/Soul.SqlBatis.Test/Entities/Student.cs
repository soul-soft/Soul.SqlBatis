using System;
using Soul.SqlBatis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Entities
{	
	/// <summary>
    /// 
    /// </summary>
	[Table("student")]
	public partial class Student
	{
		/// <summary>
		/// 
		/// </summary>
		[Column("Address")]
		public Address Address { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("CreationTime")]
		public DateTime? CreationTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("FirstName")]
		public string FirstName { get; set; }
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