using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleSerial {

	/// <summary>
	/// This class serves as the main point of logic for the ShelfRokr.
	/// </summary>
	internal class MainProgram {

		#region Singleton

		private static MainProgram m_instance;

		public static MainProgram Instance {
			get { return m_instance ?? ( m_instance = new MainProgram() ); }
		}

		#endregion Singleton

		public static string jsonFile = @"C:\ShelfRokr\config\videoConfig.json";

		[STAThread]
		private static void Main() {
			ArduinoParser.Instance.ProductPickUpEvent += Instance.OnProductPickUp;
			ArduinoParser.Instance.ProductPutDownEvent += Instance.OnProductPutDown;

			//            VideoConfigs temp = new VideoConfigs();
			//            File.WriteAllText( jsonFile, JsonConvert.SerializeObject( temp ) );

			while ( true ) {
				Thread.Sleep( 1 ); // TODO not needed with windows form app
			}
		}

		private void OnProductPickUp( int slotID ) {
			Console.WriteLine( "Picked up " + slotID );
		}

		private void OnProductPutDown( int slotID ) {
			Console.WriteLine( "Put down " + slotID );
		}
	}
}