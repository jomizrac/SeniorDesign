﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
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

		[STAThread]
		private static void Main() {
			CheckAWSCredentialsPresent();

			ShelfInventory.Instance.UpdateSlot( 0, new Product( "product 0", 0 ) );
			ShelfInventory.Instance.UpdateSlot( 1, new Product( "product 1", 1 ) );
			ShelfInventory.Instance.UpdateSlot( 2, new Product( "product 2", 2 ) );
			ShelfInventory.Instance.UpdateSlot( 3, new Product( "product 3", 3 ) );
			ShelfInventory.Instance.UpdateSlot( 4, new Product( "product 4", 4 ) );
			ShelfInventory.Instance.UpdateSlot( 5, new Product( "product 5", 5 ) );
			ShelfInventory.Instance.UpdateSlot( 6, new Product( "product 6", 6 ) );

			LocalStorage.Instance.SyncVideos();

			ArduinoParser.Instance.ProductPickUpEvent += Instance.OnProductPickUp;
			ArduinoParser.Instance.ProductPutDownEvent += Instance.OnProductPutDown;

			//            VideoConfigs temp = new VideoConfigs();
			//            File.WriteAllText( jsonFile, JsonConvert.SerializeObject( temp ) );

			while ( true ) {
				Thread.Sleep( 1 ); // TODO not needed with windows form app
			}
		}

		private static void CheckAWSCredentialsPresent() {
			string credentials = ConfigurationManager.AppSettings["AWSProfilesLocation"];
			if ( !File.Exists( credentials ) ) {
				string directory = Path.GetDirectoryName( credentials );
				Directory.CreateDirectory( directory );

				using ( StreamWriter sw = File.CreateText( credentials ) ) {
					sw.WriteLine( "[default]" );
					sw.WriteLine( "aws_access_key_id=" );
					sw.WriteLine( "aws_secret_access_key=" );
				}

				Console.WriteLine( "AWS credentials file has not been setup. Please configure the following file: " + credentials );
				Console.ReadKey(); // Wait for user input
				return;
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