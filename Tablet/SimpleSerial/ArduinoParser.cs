using System;
using System.Collections.Generic;
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

		private const string PickUp = "u";
		private const string PutDown = "d";
		private SerialPort serialPort = new SerialPort();
		private StringBuilder buffer = new StringBuilder();

		// Communication protocol
		private Regex regex = new Regex( @"^\d+^[a-zA-Z]+" ); // 1+ digits followed by 1+ alpha letters

		/// <summary>
		/// Default constructor.  This will be called immediately upon program startup from MainLoop.cs.
		/// </summary>
		public ArduinoParser() {
			serialPort.PortName = AutodetectArduinoPort() ?? "COM4";
			serialPort.BaudRate = 9600;
			serialPort.DataReceived += new SerialDataReceivedEventHandler( OnDataReceived );
			serialPort.Open();
		}

		private string AutodetectArduinoPort() {
			var connectionScope = new ManagementScope();
			var serialQuery = new SelectQuery( "SELECT * FROM Win32_SerialPort" );
			var searcher = new ManagementObjectSearcher( connectionScope, serialQuery );

			try {
				foreach ( var item in searcher.Get() ) {
					var desc = item["Description"].ToString();
					var deviceId = item["DeviceID"].ToString();

					if ( desc.Contains( "Arduino" ) ) {
						return deviceId;
					}
				}
			}
			catch ( ManagementException ) {
				// NOOP
			}

			return null;
		}

		private void OnDataReceived( object sender, SerialDataReceivedEventArgs e ) {
			// Append the new data to the buffer
			buffer.Append( serialPort.ReadExisting() );

			// See if we have sufficient data in the buffer to parse a complete telegram
			Match match;
			do {
				match = regex.Match( buffer.ToString() );
				if ( match.Success ) {
					ParseTelegram( match.Captures[0].Value );
					buffer.Remove( match.Captures[0].Index, match.Captures[0].Length );
				}
			} while ( match.Success );
		}

		// TODO list:
		// figure out how to handle losing random bytes from arduino
		// move consts into app config file

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