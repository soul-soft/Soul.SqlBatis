using System.Data;

namespace Soul.SqlBatis
{
    public class DbCommandOptions
    {
        public CommandType CommandType { get; set; } = CommandType.Text;
        public int CommandTimeout { get; set; } = 30;
    }
}
