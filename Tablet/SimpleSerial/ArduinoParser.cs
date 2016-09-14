using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSerial {

	internal class ArduinoParser {

		public delegate void ProductPickedUp( int productID );

		public delegate void ProductPlacedDown( int productID );

		public event ProductPickedUp ProductPickedUpEvent;

		public event ProductPlacedDown ProductPlacedDownEvent;

		#region Singleton

		private static ArduinoParser m_instance;

		public static ArduinoParser Instance {
			get { return m_instance ?? ( m_instance = new ArduinoParser() ); }
		}

		#endregion Singleton
	}
}