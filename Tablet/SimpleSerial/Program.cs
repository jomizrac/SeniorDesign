using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SimpleSerial {

	internal static class Program {

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void DeprecatedMain() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			Application.Run( new Form1() );
		}
	}
}