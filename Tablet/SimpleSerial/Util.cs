using System;

namespace SimpleSerial {

	internal class Util {

		public static void LogSuccess( string message, bool addNewLine = true ) {
			Log( "SUCCESS", message, addNewLine );
		}

		public static void LogWarning( string message, bool addNewLine = true ) {
			Log( "WARNING", message, addNewLine );
		}

		public static void LogError( string message, bool addNewLine = true ) {
			Log( "ERROR", message, addNewLine );
		}

		/// <summary>
		/// Prefixes a string with the current timestamp and prints it with Console.WriteLine.
		/// </summary>
		/// <param name="message"></param>
		private static void Log( string successType, string message, bool addNewLine ) {
			string timestamp = DateTime.Now.ToString( "h:mm:ss tt" );
			string log = timestamp + " " + successType + ": " + message;
			if ( addNewLine ) {
				Console.WriteLine( log );
			}
			else {
				Console.Write( log );
			}
		}
	}
}