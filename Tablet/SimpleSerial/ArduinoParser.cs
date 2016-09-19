using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleSerial {

	/// <summary>
	/// This class is responsible for reading serial output from the Arduino and converting it into more intelliglbe events.
	/// </summary>
	internal class ArduinoParser {

		#region global Variables

		private SerialPort currentPort;
		private bool portFound;

		#endregion global Variables

		#region Events

		public delegate void ProductPickUp( int productID );

		public delegate void ProductPutDown( int productID );

		public event ProductPickUp ProductPickUpEvent;

		public event ProductPutDown ProductPutDownEvent;

		#endregion Events

		#region Singleton

		private static ArduinoParser m_instance;

		public static ArduinoParser Instance {
			get { return m_instance ?? ( m_instance = new ArduinoParser() ); }
		}

		#endregion Singleton

		public void Main() {
			currentPort.Open();
			int productASCII = 0;
			char upOrDown; //is it a pick up or put down, either 'u' or 'd'
						   //guess which is which

			while ( true )    //while loop only ends when program ends
			{
				upOrDown = '\0';
				productASCII = 0;
				if ( currentPort.BytesToRead != 0 ) {
					productASCII = currentPort.ReadByte(); //If there is something in the read buffer it is the product ID
					upOrDown = (char)currentPort.ReadByte(); //The next character is pick up or put down
					if ( upOrDown == 'u' ) { //it's up
						ProductPickUpEvent( productASCII ); //picked up event
					}
					else if ( upOrDown == 'd' ) { //it's down
						ProductPutDownEvent( productASCII ); //placed down event
					}
					else {
						//TODO Error handling
					}
				}
			}
		}

		//This sets up the communication between the arduino and the tablet
		private void SetComPort() {
			try {
				string[] ports = SerialPort.GetPortNames();
				foreach ( string port in ports ) {
					currentPort = new SerialPort( port, 9600 );
					if ( DetectArduino() ) {
						portFound = true;
						break;
					}
					else {
						portFound = false;
					}
				}
			}
			catch ( Exception e ) {
			}
		}
	}
}