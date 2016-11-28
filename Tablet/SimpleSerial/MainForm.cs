using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace SimpleSerial {

	public partial class MainForm : Form {
		private int last = 1;
		private LogIn login = new LogIn();

		public MainForm() {
			InitializeComponent();

			//			Button myTwentyFivePercentOpaqueButton = new Button();
			//			myTwentyFivePercentOpaqueButton.Opacity = new Double();
			//			myTwentyFivePercentOpaqueButton.Opacity = 0.25;

			//			panel1.BackColor = Color.FromArgb( 200, Color.Black );

			// TopMost = true;
			FormBorderStyle = FormBorderStyle.None;
			WindowState = FormWindowState.Maximized;
		}

		private void axWindowsMediaPlayer1_Enter( object sender, EventArgs e ) {
		}

		private void button1_Click( object sender, EventArgs e ) {
			last = 1;
		}

		private void button2_Click( object sender, EventArgs e ) {
			if ( last == 1 || last == 2 ) {
				last = 2;
			}
			else {
				last = 0;
			}
		}

		private void button3_Click( object sender, EventArgs e ) {
			if ( last == 2 || last == 3 ) {
				last = 3;
			}
			else {
				last = 0;
			}
		}

		private void button4_Click( object sender, EventArgs e ) {
			if ( last == 3 ) {
				login.Show();
				login.BringToFront();
			}

			last = 0;
		}
	}
}