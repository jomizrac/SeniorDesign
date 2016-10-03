using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleSerial
{

    internal class VideoManager
    {

        #region Singleton

        private static VideoManager m_instance;
        private WMPLib.WindowsMediaPlayer Player;

        private bool playing = false;

        public static VideoManager Instance
        {
            get { return m_instance ?? (m_instance = new VideoManager()); }
        }

        #endregion Singleton
        

        //public static string jsonFile = @"C:\ShelfRokr\config\videoConfig.json";
        public static string behavior = ConfigurationManager.AppSettings["videoConfig"];
        public List<Product> playables = new List<Product>();


        public VideoManager()
        {
            ArduinoParser.Instance.ProductPickUpEvent += Instance.OnProductPickup;
            ArduinoParser.Instance.ProductPutDownEvent += Instance.OnProductPutDown;
        }

        private void PlayVideos()
        {
            while (playables[0] != null)
            {
                string temp = LocalStorage.videoDirectory;
                string prodID = playables[0].productID.ToString();
                PlayFile(temp + prodID + LocalStorage.videoFileExtension);
                playing = true;
                while (playing)
                {
                    if (playables[0].status == Product.Status.PickedUp)
                    {
                        Player_PlayStateChange(8);
                    }
                }
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

        private void Form1_Load(object sender, System.EventArgs e)
        {
            // TODO  Insert a valid path in the line below.
            //			PlayFile( LocalStorage.Instance.videoDirectory + playables[0].productID() + LocalStorage.Instance.fileExtension ));
        }

        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
            {
                playing = false;
                //				this.Close();
            }
        }

        private void Player_MediaError(object pMediaObject)
        {
            //			MessageBox.Show( "Cannot play media file." );
            //			this.Close();
        }

        private void OnProductPickup(int slotID)
        {
            Product current = ShelfInventory.Instance.shelfSlots[slotID];
            current.status = Product.Status.PickedUp;
   			playables.Add( current );
        }

        private void OnProductPutDown(int slotID)
        {
            Product current = ShelfInventory.Instance.shelfSlots[slotID];
            current.status = Product.Status.PutDown;
            playables.Remove(current);

        }
    }
}