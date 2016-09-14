using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSerial {

	/// <summary>
	/// This class is responsible for reading serial output from the Arduino and converting it into more intelliglbe events.
	/// </summary>
	internal class ArduinoParser {

		#region Events

		public delegate void ProductPickedUp( int productID );

		public delegate void ProductPlacedDown( int productID );

		public event ProductPickedUp ProductPickedUpEvent;

		public event ProductPlacedDown ProductPlacedDownEvent;

		#endregion Events

		#region Singleton

		private static ArduinoParser m_instance;

		public static ArduinoParser Instance {
			get { return m_instance ?? ( m_instance = new ArduinoParser() ); }
		}

		#endregion Singleton
	}
}