using System;
using System.Collections.Generic;
using System.Text;

namespace Soul.SqlBatis
{
	public class DbCommand
	{
		public string CommandText { get; }

		public Dictionary<string,object> Parameters { get; set; }
	}
}
