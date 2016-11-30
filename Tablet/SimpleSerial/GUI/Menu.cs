using SimpleSerial.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleSerial {

	public partial class Menu : Form {

		public Menu() {
			InitializeComponent();
		}

		private void button2_Click( object sender, EventArgs e ) {
			VideoInstr behavior = new VideoInstr();
			behavior.Show();
			behavior.BringToFront();
			this.Close();
		}

		private void button1_Click( object sender, EventArgs e ) {
			ProductSwitch prodSwitch = new ProductSwitch();
			prodSwitch.Show();
			prodSwitch.BringToFront();
			this.Close();
		}

		private void button3_Click( object sender, EventArgs e ) {
			LogIn login = new LogIn();
			login.Show();
			login.BringToFront();
			this.Close();
		}

		private void splitContainer2_Panel2_Paint( object sender, PaintEventArgs e ) {
		}

		private void splitContainer1_Panel2_Paint( object sender, PaintEventArgs e ) {
		}

		private void Menu_Load( object sender, EventArgs e ) {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        }

		private void Menu_Deactivate( object sender, EventArgs e ) {
			this.Close();
		}

		private void splitContainer2_Panel1_Paint( object sender, PaintEventArgs e ) {
		}

		private void button4_Click( object sender, EventArgs e ) {
			LocalStorage.Instance.SyncVideos();
		}

        private void button1_Click_1(object sender, EventArgs e)
        {
            ProductSwitch pSwitch = new ProductSwitch();
            pSwitch.Show();
            pSwitch.BringToFront();
            this.Close();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            VideoInstr vid = new VideoInstr();
            vid.Show();
            vid.BringToFront();
            this.Close();
        }
    }
}