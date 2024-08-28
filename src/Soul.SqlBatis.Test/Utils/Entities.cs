

using System;
using Soul.SqlBatis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Entities
{	

	/// <summary>
    /// 
    /// </summary>
	[Table("mysql_data_types")]
	public partial class MysqlDataTypes
	{
		/// <summary>
		/// 
		/// </summary>
		[Column("bigint_col")]
		public long? BigintCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("blob_col")]
		public byte[]? BlobCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("char_col")]
		public string CharCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("date_col")]
		public DateTime? DateCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("datetime_col")]
		public DateTime? DatetimeCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("decimal_col")]
		public decimal? DecimalCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("double_col")]
		public double? DoubleCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("enum_col")]
		public string EnumCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("float_col")]
		public float? FloatCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Key][Identity][Column("id")]
		public int? Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("int_col")]
		public int? IntCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("json_col")]
		public KeyJsonValue JsonCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("mediumint_col")]
		public int? MediumintCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("set_col")]
		public string SetCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("smallint_col")]
		public short? SmallintCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("text_col")]
		public string TextCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("time_col")]
		public TimeSpan? TimeCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("timestamp_col")]
		public DateTime? TimestampCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("tinyint_col")]
		public short? TinyintCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("varchar_col")]
		public string VarcharCol { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("year_col")]
		public int? YearCol { get; set; }
	}

	/// <summary>
    /// 
    /// </summary>
	[Table("student")]
	public partial class Student
	{
		/// <summary>
		/// 
		/// </summary>
		[Key][Identity][Column("id")]
		public int? Id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("name")]
		public string Name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("num0")]
		public bool? Num0 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("num1")]
		public short? Num1 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("num2")]
		public short? Num2 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("num3")]
		public int? Num3 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[Column("num4")]
		public long? Num4 { get; set; }
	}
}

