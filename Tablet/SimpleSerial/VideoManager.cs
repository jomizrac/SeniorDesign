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
using System.Threading.Tasks;

namespace SimpleSerial {

	internal class VideoManager {

		//public static string jsonFile = @"C:\ShelfRokr\config\videoConfig.json";
		public static string behavior = ConfigurationManager.AppSettings["videoConfig"];

		public List<Product> playables = new List<Product>();
		private WMPLib.WindowsMediaPlayer Player;

		private bool playing = false;

		#region Singleton

		private static VideoManager m_instance;

		public static VideoManager Instance {
			get { return m_instance ?? ( m_instance = new VideoManager() ); }
		}

		#endregion Singleton

		private string Directory = LocalStorage.videoDirectory;

		public VideoManager() {
			new Thread( () => Initialize() ).Start();

			//			var delay = Task.Run( async () => {
			//				await Task.Delay( 2500 );
			//				MainProgram.form.PlayTest( LocalStorage.Instance.GetFilePathForProduct( 1 ) );
			//			} );
		}

		private void Initialize() {
			// TODO temp unregistering from events to prevent duplicate registration
			ArduinoParser.Instance.ProductPickUpEvent -= Instance.OnProductPickup;
			ArduinoParser.Instance.ProductPutDownEvent -= Instance.OnProductPutDown;
			ArduinoParser.Instance.ProductPickUpEvent += Instance.OnProductPickup;
			ArduinoParser.Instance.ProductPutDownEvent += Instance.OnProductPutDown;

			Player = new WMPLib.WindowsMediaPlayer();
			Player.PlayStateChange -= new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler( Player_PlayStateChange );
			Player.MediaError -= new WMPLib._WMPOCXEvents_MediaErrorEventHandler( Player_MediaError );
			Player.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler( Player_PlayStateChange );
			Player.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler( Player_MediaError );
		}

		private void PlayFile( string url ) {
			Player.URL = url;
			Player.controls.play();
		}

		private void Form1_Load( object sender, System.EventArgs e ) {
			// TODO  Insert a valid path in the line below.
			//			PlayFile( LocalStorage.Instance.videoDirectory + playables[0].productID() + LocalStorage.Instance.fileExtension ));
		}

		private void Player_PlayStateChange( int NewState ) {
			if ( (WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsMediaEnded && ( config == 2 )) {
                MainProgram.form.PlayVideo(LocalStorage.Instance.GetFilePathForProduct(playables[current]));
                playables.Remove( playables[0]) ;
				//				this.Close();
			}
		}

		private void Player_MediaError( object pMediaObject ) {
			//			MessageBox.Show( "Cannot play media file." );
			//			this.Close();
		}

		private void OnProductPickup( int slotID ) {
			int config = 1;
			Product current = ShelfInventory.Instance.shelfSlots[slotID];
			current.status = Product.Status.PickedUp;
			if ( config == 1 ) {
				//Play current video immediately
				MainProgram.form.PlayVideo( LocalStorage.Instance.GetFilePathForProduct( slotID ) );
			}
			if ( config == 2 ) {
                if ( !playables.Any() )
                {
                    MainProgram.form.PlayVideo(LocalStorage.Instance.GetFilePathForProduct(slotID));

                } else
                {
                    playables.Add(current);
                }
			}
		}

		private void OnProductPutDown( int slotID ) {
			Product current = ShelfInventory.Instance.shelfSlots[slotID];
			current.status = Product.Status.PutDown;
			playables.Remove( current );
		}
	}
}