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
		[Column("Age")]
		public string Age { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("CreationTime")]
		public string Creationtime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("FirstName")]
		public string Firstname { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Key][Column("Id")]
		public int Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Money")]
		public double? Money { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("Name")]
		public string Name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("RowVersion")]
		public string Rowversion { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("State")]
		public StudentState? State { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("BinaryData")]
		public byte[] BinaryData { get; set; }

    }
}
