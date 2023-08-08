using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soul.SqlBatis.Test
{
    [Table("students")]
    public class Student
    {
        public uint Id { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
