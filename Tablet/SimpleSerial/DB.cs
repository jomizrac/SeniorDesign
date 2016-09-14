using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSerial {

	internal class DB {
		private static DB m_instance;

		public static DB Instance {
			get { return m_instance ?? ( m_instance = new DB() ); }
		}
	}
}