using System.Collections.Generic;

namespace Soul.SqlBatis
{
    public class DbCommand
    {
        public string CommandText { get; }

        public Dictionary<string, object> Parameters { get; set; }
    }
}
