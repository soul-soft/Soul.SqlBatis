using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Test.Entities
{
    [Table("students")]
    public class Student
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("age")]
        public int? Age { get; set; }

        [Column("category_id")]
        public int? CategoryId { get; set; }

        [Column("category_sec_id")]
        public int? CategorySecId { get; set; }

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
