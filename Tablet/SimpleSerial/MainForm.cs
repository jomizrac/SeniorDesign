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

		public MainForm() {
			InitializeComponent();

			// TopMost = true;
			//			TopLevel = true;
			FormBorderStyle = FormBorderStyle.None;
			WindowState = FormWindowState.Maximized;

			var subForm = new ButtonForm();
			//			subForm.TopLevel = false;
			//			subForm.TopMost = true;
			subForm.FormBorderStyle = FormBorderStyle.None;
			subForm.WindowState = FormWindowState.Maximized;
			//			Controls.Add( subForm );
			subForm.Visible = true;
			subForm.BringToFront();
		}

		private void axWindowsMediaPlayer1_Enter( object sender, EventArgs e ) {
		}
	}
}