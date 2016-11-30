using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleSerial.GUI {

	public partial class VideoConf : Form {
		public string behavior = "";

		public VideoConf( string behavior ) {
			this.behavior = behavior;
			InitializeComponent();
		}

		private void label1_Click( object sender, EventArgs e ) {
		}

		private void button1_Click( object sender, EventArgs e ) {
			if ( behavior == "queued" ) {
				VideoManager.Instance.SetPlaybackMethod( VideoManager.PlaybackMethod.Queued );
			}
			else if ( behavior == "interrupt" ) {
				VideoManager.Instance.SetPlaybackMethod( VideoManager.PlaybackMethod.Immediate );
			}
			else {
				LogIn login = new LogIn();
				login.Show();
                login.BringToFront();
				this.Close();
			}
			Menu menu = new SimpleSerial.Menu();
			menu.Show();
            menu.BringToFront();
			this.Close();
		}

		private void VideoConf_Deactivate( object sender, EventArgs e ) {
			this.Close();
		}

        private void VideoConf_Load(object sender, EventArgs e)
        {

        }
    }
}