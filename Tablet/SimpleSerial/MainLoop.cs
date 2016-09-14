using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSerial {

	/// <summary>
	/// This class serves as the main point of logic for the ShelfRokr.
	/// </summary>
	internal class MainLoop {

		#region Singleton

		private static MainLoop m_instance;

		public static MainLoop Instance {
			get { return m_instance ?? ( m_instance = new MainLoop() ); }
		}

		#endregion Singleton

		private static void Main() {
			ArduinoParser.Instance.ProductPickedUpEvent += Instance.OnProductPickup;

			while ( true ) {
				// NOOP
			}
		}

		private void OnProductPickup( int productID ) {
		}
	}
}