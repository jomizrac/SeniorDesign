using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSerial {

	internal class Util {

		/// <summary>
		/// Prefixes a string with the current timestamp and prints it with Console.WriteLine.
		/// </summary>
		/// <param name="message"></param>
		public static void Log( string message, bool addNewLine = true ) {
			string log = DateTime.Now.ToString( "h:mm:ss tt" ) + ": " + message;
			if ( addNewLine ) {
				Console.WriteLine( log );
			}
			else {
				Console.Write( log );
			}
		}
	}
}