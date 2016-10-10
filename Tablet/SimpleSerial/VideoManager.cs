using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleSerial {

	internal class VideoManager {

		#region Singleton

		private static VideoManager m_instance;
		private WMPLib.WindowsMediaPlayer Player;

		private bool playing = false;

		public static VideoManager Instance {
			get { return m_instance ?? ( m_instance = new VideoManager() ); }
		}

		#endregion Singleton

		//public static string jsonFile = @"C:\ShelfRokr\config\videoConfig.json";
		public static string behavior = ConfigurationManager.AppSettings["videoConfig"];
        private string Directory = LocalStorage.videoDirectory;
        public List<Product> playables = new List<Product>();

		public VideoManager() {
			new Thread( () => Initialize() ).Start();
		}

		private void Initialize() {
			ArduinoParser.Instance.ProductPickUpEvent += Instance.OnProductPickup;
			ArduinoParser.Instance.ProductPutDownEvent += Instance.OnProductPutDown;
		}

		private void PlayVideos() {
			int current = 0;
			while ( playables[current] != null ) {
    			PlayFile( LocalStorage.Instance.GetFilePathForProduct(playables[current]) );
                current++;
            }
		}

		private void PlayFile( String url ) {
			Player = new WMPLib.WindowsMediaPlayer();
			Player.PlayStateChange +=
				new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler( Player_PlayStateChange );
			Player.MediaError +=
				new WMPLib._WMPOCXEvents_MediaErrorEventHandler( Player_MediaError );
			Player.URL = url;
			Player.controls.play();
		}

		private void Form1_Load( object sender, System.EventArgs e ) {
			// TODO  Insert a valid path in the line below.
			//			PlayFile( LocalStorage.Instance.videoDirectory + playables[0].productID() + LocalStorage.Instance.fileExtension ));
		}

		private void Player_PlayStateChange( int NewState ) {
			if ( (WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped ) {
				playing = false;
				//				this.Close();
			}
		}

		private void Player_MediaError( object pMediaObject ) {
			//			MessageBox.Show( "Cannot play media file." );
			//			this.Close();
		}

		private void OnProductPickup( int slotID ) {
			// Temp debugging
			Console.WriteLine( "trying to play video " + slotID );
			//Process.Start( LocalStorage.Instance.GetFilePathForProduct( slotID ) );
            int config = 1;
			Product current = ShelfInventory.Instance.shelfSlots[slotID];
			current.status = Product.Status.PickedUp;
			playables.Add( current );
            if (config == 1)
            {
                //Play current video immediately
                string prodID = current.productID.ToString();
                PlayFile(LocalStorage.Instance.GetFilePathForProduct(slotID));
            }
            if (config == 2)
            {
                //Play queue of videos
                PlayVideos();
            }

        }

		private void OnProductPutDown( int slotID ) {
			Product current = ShelfInventory.Instance.shelfSlots[slotID];
			current.status = Product.Status.PutDown;
			playables.Remove( current );
		}
	}
}