using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Test.Entities
{
    [Table("student",Schema = "public")]
    public class Student
    {
        [Key]
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

        public void SetName(string name)
        {
            this.Name = name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is not Student)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            var that = (Student)obj;

            return that.Id == this.Id;
        }
    }

    public enum Gender
    {
        男,
        女
    }
}
