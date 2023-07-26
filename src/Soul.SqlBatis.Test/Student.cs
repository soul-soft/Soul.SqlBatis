using System.ComponentModel.DataAnnotations;

namespace Soul.SqlBatis.Test
{
    public class Student
    {
        [Key]
        public int? Id { get; set; }
        public string Name { get; set; }
    }
}
