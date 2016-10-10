using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SimpleSerial {

	/// <summary>
	/// This class is responsible for reading serial output from the Arduino and converting it into more intelliglbe events.
	/// </summary>
	internal class ArduinoParser {

		#region Singleton

		private static ArduinoParser m_instance;

		public static ArduinoParser Instance {
			get { return m_instance ?? ( m_instance = new ArduinoParser() ); }
		}

		#endregion Singleton

		#region Events

		public delegate void ProductPickUp( int slotID );

		public delegate void ProductPutDown( int slotID );

		public event ProductPickUp ProductPickUpEvent;

		public event ProductPutDown ProductPutDownEvent;

		#endregion Events

		private const string PickUp = "U";
		private const string PutDown = "D";
		private SerialPort serialPort = new SerialPort();
		private StringBuilder buffer = new StringBuilder();

		// Communication protocol
		private Regex telegramDelimMatcher = new Regex( @"^.*(\n|\r|\r\n)" ); // Match any chars followed by a newline

		public ArduinoParser() {
			serialPort.PortName = AutodetectArduinoPort() ?? ConfigurationManager.AppSettings["defaultPort"];
			serialPort.BaudRate = int.Parse( ConfigurationManager.AppSettings["baudRate"] );
			serialPort.DataReceived += new SerialDataReceivedEventHandler( OnDataReceived );
			serialPort.Open();
			Console.WriteLine( "Successfully opened port: " + serialPort.PortName );
		}

		//Checks which port the Arduino is connected to
		private string AutodetectArduinoPort() {
			var connectionScope = new ManagementScope();
			var serialQuery = new SelectQuery( "SELECT * FROM Win32_SerialPort" );
			var searcher = new ManagementObjectSearcher( connectionScope, serialQuery );

			try {
				foreach ( var item in searcher.Get() ) {
					var desc = item["Description"].ToString();
					var deviceId = item["DeviceID"].ToString();

					if ( desc.Contains( "Arduino" ) ) {
						Console.WriteLine( "Autodetected Arduino on port: " + deviceId );
						return deviceId;
					}
				}
			}
			catch ( ManagementException ) {
				// NOOP
			}

			Console.WriteLine( "Unable to autodetect Arduino port" );
			return null;
		}

		private void OnDataReceived( object sender, SerialDataReceivedEventArgs e ) {
			// Append the new data to the buffer
			string message = serialPort.ReadExisting();
			buffer.Append( message );

			// See if we have sufficient data in the buffer to parse a complete telegram
			Match match;
			do {
				match = telegramDelimMatcher.Match( buffer.ToString() );
				if ( match.Success ) {
					ParseTelegram( match.Captures[0].Value );
					buffer.Remove( match.Captures[0].Index, match.Captures[0].Length );
				}
			} while ( match.Success );
		}

		//Parses the message received to check which type of event it is
		private void ParseTelegram( string telegram ) {
			var numAlpha = new Regex( "(?<Numeric>[0-9]*)(?<Alpha>[a-zA-Z]*)" );
			var match = numAlpha.Match( telegram );
			var alpha = match.Groups["Alpha"].Value;
			var num = match.Groups["Numeric"].Value;
			var slotID = int.Parse( num );

			if ( alpha == PickUp ) {
				ProductPickUpEvent( slotID );
			}
			else if ( alpha == PutDown ) {
				ProductPutDownEvent( slotID );
			}
			else {
				Console.WriteLine( "Unrecognized alpha string: " + alpha );
			}
		}
	}
}