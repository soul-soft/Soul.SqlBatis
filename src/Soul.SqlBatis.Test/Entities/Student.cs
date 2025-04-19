using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Test.Entities
{
    [Table("student",Schema = "public")]
    public class Student
    {
        [Key][Identity]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("age")]
        public int? Age { get; set; }

        [Key]
        [Column("tenant_id")]
        public int? TenantId { get; set; }

        [Column("dep_ids")]
        public int[] DepIds { get; set; } = default!;

        [Column("gender")]
        public Gender? Gender { get; set; }
    }

    public enum Gender
    {
        男,
        女
    }
}
