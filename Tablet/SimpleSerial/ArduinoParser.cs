using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSerial {

	internal class ArduinoParser {

		public delegate void ProductPickedUp( int productID );

		public event ProductPickedUp ProductPickedUpEvent;

		private static ArduinoParser m_instance;

		public static ArduinoParser Instance {
			get { return m_instance ?? ( m_instance = new ArduinoParser() ); }
		}

		private void SampleUsage() {
			ArduinoParser.Instance.ProductPickedUpEvent += OnPickup;
		}

		private void OnPickup( int productID ) {
			// Do something with productID
		}
	}
}