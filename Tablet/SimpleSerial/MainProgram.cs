using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
			ShelfInventory.Instance.UpdateSlot( 0, new Product( "product 0", "0", 0 ) );
			ShelfInventory.Instance.UpdateSlot( 1, new Product( "product 1", "1", 1 ) );
			ShelfInventory.Instance.UpdateSlot( 2, new Product( "product 2", "2", 2 ) );
			ShelfInventory.Instance.UpdateSlot( 3, new Product( "product 3", "3", 3 ) );
			ShelfInventory.Instance.UpdateSlot( 4, new Product( "product 4", "4", 4 ) );
			ShelfInventory.Instance.UpdateSlot( 5, new Product( "product 5", "5", 5 ) );
			ShelfInventory.Instance.UpdateSlot( 6, new Product( "product 6", "6", 6 ) );
            //write new config file
            VideoConfigs config = new VideoConfigs();
            config.behavior = VideoConfigs.Behavior.Queued;
            Instance.Serialize(config);
            //serialize to json

			// Pull any missing videos
			LocalStorage.Instance.SyncVideos();

			// Initialize any singletons that have not been called yet
			var ard = ArduinoParser.Instance;
            var led = LEDManager.Instance;
			var vm = VideoManager.Instance;
			var db = DB.Instance;
			var log = Logger.Instance;

			Application.Run( Instance.Form );
			// Warning: no code after Application.Run()'s while-loop will be reached!
		}

        private void Serialize(VideoConfigs config)
        {
            string videoConfigFile = ConfigurationManager.AppSettings["videoConfig"];

            if (!File.Exists(videoConfigFile))
            {
                string directory = Path.GetDirectoryName(videoConfigFile);
                Directory.CreateDirectory(directory);
                File.Create(videoConfigFile);
            }

            File.WriteAllText(videoConfigFile, JsonConvert.SerializeObject(config));
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

				Console.WriteLine( "AWS credentials file has not been setup. Please configure the following file: " + credentials );
				Console.ReadKey(); // Wait for user input
				return false;
			}
			return true;
		}
	}
}