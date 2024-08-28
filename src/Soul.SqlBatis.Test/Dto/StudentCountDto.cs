using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soul.SqlBatis.Test.Dto
{
    public class StudentCountDto
    {
        [Column("id")]
        public int? Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("age")]
        public int? Age { get; set; }
        [Column("gender")]
        public string Gender { get; set; }
        [Column("class")]
        public string Class { get; set; }
        [Column("avg")]
        public decimal? Avg { get; set; }
    }
}
