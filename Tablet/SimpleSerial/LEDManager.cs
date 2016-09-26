using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSerial {

	internal class LEDManager {

		#region Singleton

		private static LEDManager m_instance;

		public static LEDManager Instance {
			get { return m_instance ?? ( m_instance = new LEDManager() ); }
		}

		#endregion Singleton
        //if product picked up, light until put back 
        //if vid playing, flash maybe?
        //how do we communicate to the arduino to turn on the light?
	}
}