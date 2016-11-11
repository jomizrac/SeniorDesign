using System.Configuration;
using System.IO.Ports;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleSerial {

	/// <summary>
	/// This class is responsible for reading serial output from the Arduino and converting it into more intelliglbe events.
	/// </summary>
	internal class ArduinoParser {

		#region Singleton

		private static object LOCK = new object();

		private static ArduinoParser m_instance;

		public static ArduinoParser Instance {
			get {
				lock ( LOCK ) {
					return m_instance ?? ( m_instance = new ArduinoParser() );
				}
			}
		}

		#endregion Singleton

		#region Events

		public delegate void SlotPickUp( int slotIdx );

		public delegate void SlotPutDown( int slotIdx );

		public event SlotPickUp SlotPickUpEvent;

		public event SlotPutDown SlotPutDownEvent;

		#endregion Events

		private const string PickUp = "U";
		private const string PutDown = "D";
		private SerialPort serialPort = new SerialPort();
		private StringBuilder buffer = new StringBuilder();

		// Communication protocol
		private Regex telegramDelimMatcher = new Regex( @"^.*(\r\n|\n|\r)" ); // Match any chars followed by a newline

		private ArduinoParser() {
			serialPort.PortName = AutodetectArduinoPort();
			serialPort.BaudRate = int.Parse( ConfigurationManager.AppSettings["baudRate"] );
			serialPort.DataReceived -= new SerialDataReceivedEventHandler( OnDataReceived );
			serialPort.DataReceived += new SerialDataReceivedEventHandler( OnDataReceived );
			serialPort.Open();
			Util.LogSuccess( "Opened port: " + serialPort.PortName );
		}

		//sending commands to the Arduino for it to exexute on the hardware
		//only sends commands for LEDs from LEDManager right now
		public void SendCommand( string command ) {
			serialPort.WriteLine( command );
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
						Util.LogSuccess( "Autodetected Arduino on port: " + deviceId );
						return deviceId;
					}
				}
			}
			catch ( ManagementException ) {
				// NOOP
			}

			string defaultPort = ConfigurationManager.AppSettings["defaultPort"];
			Util.LogWarning( "Unable to autodetect Arduino port. Reverting to default port: " + defaultPort );
			return defaultPort;
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
			var slotIdx = int.Parse( num );

			if ( alpha == PickUp ) {
				SlotPickUpEvent?.Invoke( slotIdx );
			}
			else if ( alpha == PutDown ) {
				SlotPutDownEvent?.Invoke( slotIdx );
			}
			else {
				Util.LogError( "Unrecognized alpha string: " + alpha );
			}
		}
	}
}