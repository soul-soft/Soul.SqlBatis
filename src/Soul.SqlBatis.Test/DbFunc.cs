using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soul.SqlBatis.Test
{
    public static class DbFunc
    {
        [DbFunction(Name = "count",Format = "*")]
        public static int Count()
        { 
            throw new NotImplementedException();
        }
    }
}
