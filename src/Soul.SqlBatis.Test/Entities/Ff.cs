using System;
using Soul.SqlBatis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Entities
{	
	/// <summary>
    /// VIEW
    /// </summary>
	[Table("ff")]
	public partial class Ff
	{
		/// <summary>
		/// 
		/// </summary>
		[Column("1")]
		public int 1 { get; set; }
	}
}
