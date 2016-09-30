using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

		public static string jsonFile = @"C:\ShelfRokr\config\videoConfig.json";

		private static void Main() {
			ArduinoParser.Instance.ProductPickUpEvent += Instance.OnProductPickUp;
			ArduinoParser.Instance.ProductPutDownEvent += Instance.OnProductPutDown;

			//            VideoConfigs temp = new VideoConfigs();
			//            File.WriteAllText( jsonFile, JsonConvert.SerializeObject( temp ) );

			while ( true ) {
				// NOOP
			}
		}

		private void OnProductPickUp( int productID ) {
		}

		private void OnProductPutDown( int productID ) {
		}
	}
}