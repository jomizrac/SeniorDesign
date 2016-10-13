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

		private PlaybackMethod playbackMethod = PlaybackMethod.Queued;
		private List<Product> queue = new List<Product>();

		private int flag;

		/// <summary> Just for syntax convenience. </summary>
		public AxWindowsMediaPlayer Player { get { return MainProgram.Instance.Form.Player; } }

		// public static string jsonFile = @"C:\ShelfRokr\config\videoConfig.json";
		// private static string behavior = ConfigurationManager.AppSettings["videoConfig"];

		#region Singleton

		private static VideoManager m_instance;

		public static VideoManager Instance {
			get { return m_instance ?? ( m_instance = new VideoManager() ); }
		}

		#endregion Singleton

		private VideoManager() {
			new Thread( () => Initialize() ).Start();
			new Thread( () => Loop() ).Start();
		}

		private void Initialize() {
			ShelfInventory.Instance.ProductPickUpEvent -= Instance.OnProductPickup;
			ShelfInventory.Instance.ProductPickUpEvent += Instance.OnProductPickup;

			ShelfInventory.Instance.ProductPutDownEvent -= Instance.OnProductPutDown;
			ShelfInventory.Instance.ProductPutDownEvent += Instance.OnProductPutDown;

			Player.PlayStateChange -= new _WMPOCXEvents_PlayStateChangeEventHandler( OnPlayStateChange );
			Player.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler( OnPlayStateChange );
		}

		private void OnPlayStateChange( object sender, _WMPOCXEvents_PlayStateChangeEvent e ) {
			Console.WriteLine( "PlayStateChange [" + e.newState + "] at " + DateTime.Now.ToShortTimeString() );

			if ( playbackMethod == PlaybackMethod.Queued && e.newState == (int)WMPLib.WMPPlayState.wmppsMediaEnded && queue.Count > 0 ) {
				// Remove the just-played video
				queue.RemoveAt( 0 );

				// We cannot call Play() in here, see:
				// http://stackoverflow.com/questions/9618153/playing-two-video-with-axwindowsmediaplayer
				// http://www.dreamincode.net/forums/topic/202062-c%23-media-player-automatically-play-next-mp3/page__view__findpost__p__1322620?s=7e94316f9e27364c8007914acc29b547

				// If there's another video in the queue, play it
				Interlocked.Increment( ref flag );
				//				if ( queue.Count > 0 ) {
				//					Player.Play( queue[0] );
				//				}

				// http://stackoverflow.com/a/154803
			}
		}

		private void Loop() {
			while ( true ) {
				if ( flag > 0 ) {
					flag = 0; // No need for Interlocked due to the only other place it being modified being locked
					if ( queue.Count > 0 ) {
						Player.Play( queue[0] );
					}
				}
				Thread.Sleep( 100 );
			}
		}

		private void OnProductPickup( Product product ) {
			if ( playbackMethod == PlaybackMethod.Immediate ) {
				Player.Play( product );
			}
			else if ( playbackMethod == PlaybackMethod.Queued ) {
				// If the queue is empty (no video currently playing), go ahead and start this video
				if ( queue.Count == 0 ) {
					Player.Play( product );
				}

				// Add it to the queue, even if it's being played right away
				queue.Add( product );
			}
		}

		private void OnProductPutDown( Product product ) {
			if ( playbackMethod == PlaybackMethod.Queued ) {
				// If we put down the video that's currently playing, and there's another waiting in the queue...
				if ( queue.Count > 1 && queue[0] == product ) {
					Player.Play( queue[1] );
				}

				queue.Remove( product );
			}
		}
	}
}