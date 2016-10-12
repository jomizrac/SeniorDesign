using Amazon.S3;
using Amazon.S3.Transfer;
using AxWMPLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleSerial {

	internal class VideoManager {

		private enum PlaybackMethod { Immediate, Queued };

		private PlaybackMethod playbackMethod = PlaybackMethod.Immediate;
		private List<Product> queue = new List<Product>();

		// Convenience properties
		//		public MainForm Form { get { return MainProgram.Instance.Form; } }

		//		public AxWindowsMediaPlayer Player { get { return Form.Player; } }

		//public static string jsonFile = @"C:\ShelfRokr\config\videoConfig.json";
		//		private static string behavior = ConfigurationManager.AppSettings["videoConfig"];

		#region Singleton

		private static VideoManager m_instance;

		public static VideoManager Instance {
			get { return m_instance ?? ( m_instance = new VideoManager() ); }
		}

		#endregion Singleton

		public VideoManager() {
			new Thread( () => Initialize() ).Start();
		}

		private void Initialize() {
			ShelfInventory.Instance.ProductPickUpEvent -= Instance.OnProductPickup;
			ShelfInventory.Instance.ProductPickUpEvent += Instance.OnProductPickup;

			ShelfInventory.Instance.ProductPutDownEvent -= Instance.OnProductPutDown;
			ShelfInventory.Instance.ProductPutDownEvent += Instance.OnProductPutDown;

			MainProgram.Form.Player.PlayStateChange -= new _WMPOCXEvents_PlayStateChangeEventHandler( OnPlayStateChange );
			MainProgram.Form.Player.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler( OnPlayStateChange );
		}

		private void OnPlayStateChange( object sender, _WMPOCXEvents_PlayStateChangeEvent e ) {
			if ( playbackMethod == PlaybackMethod.Queued && e.newState == (int)WMPLib.WMPPlayState.wmppsMediaEnded && queue.Any() ) {
				MainProgram.Form.Player.Play( queue[0] );
				queue.RemoveAt( 0 );
			}
		}

		private void OnProductPickup( Product product ) {
			Console.WriteLine( "trying to play " + product );

			if ( playbackMethod == PlaybackMethod.Immediate ) {
				MainProgram.Form.Player.Play( product );
			}
			else if ( playbackMethod == PlaybackMethod.Queued ) {
				if ( !queue.Any() ) { // TODO replace this with 'if video currently playing' -- or -- add the currently playing video to the queue
					MainProgram.Form.Player.Play( product );
				}
				else {
					queue.Add( product );
				}
			}
		}

		private void OnProductPutDown( Product product ) {
			queue.Remove( product );
		}
	}
}