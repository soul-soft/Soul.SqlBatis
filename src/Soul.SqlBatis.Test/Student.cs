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

        public override bool Equals(object? obj)
        {
           if (obj == null || !(obj is Student)) 
                return false;
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            var other = (Student)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
