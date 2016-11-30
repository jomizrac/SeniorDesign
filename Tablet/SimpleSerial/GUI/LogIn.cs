using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleSerial {

	public partial class LogIn : Form {

		public LogIn() {
			InitializeComponent();
		}

		private void LogIn_Load( object sender, EventArgs e ) {
		}

		private void splitContainer1_Panel1_Paint( object sender, PaintEventArgs e ) {
		}

		private void textBox1_TextChanged( object sender, EventArgs e ) {
		}

		private void label1_Click( object sender, EventArgs e ) {
		}

		private void hello_Click( object sender, EventArgs e ) {
		}

		private void button2_Click( object sender, EventArgs e ) {
			Menu menu = new Menu();
			menu.Show();
			this.Close();
		}

		private void button1_Click( object sender, EventArgs e ) {
			this.Close();
		}

		private void LogIn_Deactivate( object sender, EventArgs e ) {
			this.Close();
		}
	}
}