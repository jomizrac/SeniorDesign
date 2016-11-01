using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace SimpleSerial {

	/// <summary>
	/// This class serves as the main point of logic for the ShelfRokr.
	/// </summary>
	internal class MainProgram {
		public MainForm Form;

		#region Singleton

		private static MainProgram m_instance;

		public static MainProgram Instance {
			get { return m_instance ?? ( m_instance = new MainProgram() ); }
		}

		#endregion Singleton

		[STAThread]
		private static void Main() {
			if ( !AWSCredentialsPresent() ) Application.Exit();

			// Setup the form for playing videos
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false ); // This must be called before MainForm is instantiated
			Instance.Form = new MainForm();

			// Establish a dummy shelf inventory
			ShelfInventory.Instance.UpdateSlot( 0, new Product( "857344005037", "CounterIntelligence", 0 ) );
			ShelfInventory.Instance.UpdateSlot( 1, new Product( "857344005020", "Ha Ha", 1 ) );
			ShelfInventory.Instance.UpdateSlot( 2, new Product( "857344005013", "ICU", 2 ) );
			ShelfInventory.Instance.UpdateSlot( 3, new Product( "857344005075", "Remember Me - Clear", 3 ) );
			ShelfInventory.Instance.UpdateSlot( 4, new Product( "857344005082", "Remember Me - Tinted", 4 ) );
			ShelfInventory.Instance.UpdateSlot( 5, new Product( "857344005051", "SOS", 5 ) );
			ShelfInventory.Instance.UpdateSlot( 6, new Product( "857344005068", "Lightening Bugs", 6 ) );

			// Pull any missing videos
			LocalStorage.Instance.SyncVideos();

			// Temp way to set playback method
			//			VideoManager.Instance.SetPlaybackMethod( VideoManager.PlaybackMethod.Queued );

			// Initialize any singletons that have not been called yet
			var ard = ArduinoParser.Instance;
			var idle = IdleDetector.Instance;
			var vm = VideoManager.Instance;
			var led = LEDManager.Instance;
			var db = Database.Instance;
			var log = Logger.Instance;

			Application.Run( Instance.Form );
			// Warning: no code after Application.Run()'s while-loop will be reached!
		}

		/// <summary> Makes sure the credentials for AWS are present in the correct file </summary>
		private static bool AWSCredentialsPresent() {
			string credentials = ConfigurationManager.AppSettings["AWSProfilesLocation"];
			if ( !File.Exists( credentials ) ) {
				string directory = Path.GetDirectoryName( credentials );
				Directory.CreateDirectory( directory );

				using ( StreamWriter sw = File.CreateText( credentials ) ) {
					sw.WriteLine( "[default]" );
					sw.WriteLine( "aws_access_key_id=" );
					sw.WriteLine( "aws_secret_access_key=" );
				}

				Util.Log( "AWS credentials file has not been setup. Please configure the following file: " + credentials );
				Console.ReadKey(); // Wait for user input
				return false;
			}
			return true;
		}
	}
}