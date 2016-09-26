using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace SimpleSerial {

	internal class VideoManager {

		#region Singleton

		private static VideoManager m_instance;
		public static VideoManager Instance {
			get { return m_instance ?? ( m_instance = new VideoManager() ); }
		}

        WMPLib.WindowsMediaPlayer Player;

        #endregion Singleton  
        private void main()
        {
            
            while (ShelfInventory.Instance.playables[0] != null)
            {
                //playvid(playables[0]);
            }
        }
        

      

        private void PlayFile(String url)
        {
            Player = new WMPLib.WindowsMediaPlayer();
            Player.PlayStateChange +=
                new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
            Player.MediaError +=
                new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
            Player.URL = url;
            Player.controls.play();
        }

        private void Form1_Load(object sender, System.EventArgs e, )
        {
            // TODO  Insert a valid path in the line below.
            PlayFile(LocalStorage.Instance.videoDirectory + playables[0].productID() + LocalStorage.Instance.fileExtension));
        }

        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
            {
                this.Close();
            }
        }

        private void Player_MediaError(object pMediaObject)
        {
            MessageBox.Show("Cannot play media file.");
            this.Close();
        }
    }
}