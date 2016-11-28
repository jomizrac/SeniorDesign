using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleSerial {

	public partial class ButtonForm : Form {
		private int last = 1;
		private LogIn login = new LogIn();

		public ButtonForm() {
			InitializeComponent();
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