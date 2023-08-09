using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Test
{
    [Table("students")]
    public class Student
    {
        [Key]
        [Identity]
        public uint Id { get; set; }
        
        public string Name { get; set; }
		[Column("first_name")]
		public string FirstName { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
