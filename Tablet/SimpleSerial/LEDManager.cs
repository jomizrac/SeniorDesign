using System.Threading;

namespace SimpleSerial {

	internal class LEDManager {

		#region Singleton

		private static LEDManager m_instance;

		public static LEDManager Instance {
			get { return m_instance ?? ( m_instance = new LEDManager() ); }
		}

		#endregion Singleton

		private LEDManager() {
			new Thread( () => Initialize() ).Start();
		}

		//register for events coming from arduino parser
		private void Initialize() {
			ArduinoParser.Instance.SlotPickUpEvent -= Instance.OnSlotPickup;
			ArduinoParser.Instance.SlotPickUpEvent += Instance.OnSlotPickup;

			ArduinoParser.Instance.SlotPutDownEvent -= Instance.OnSlotPutDown;
			ArduinoParser.Instance.SlotPutDownEvent += Instance.OnSlotPutDown;

			IdleDetector.Instance.IdleProcessEvent -= Instance.SendChaseCommand;
			IdleDetector.Instance.IdleProcessEvent += Instance.SendChaseCommand;
		}

		//activate light at a slot
		private void OnSlotPickup( int slotIdx ) {
			string command = "LED U " + slotIdx;    //build the command
			ArduinoParser.Instance.SendCommand( command );
		}

		//deactivate light at a slot
		private void OnSlotPutDown( int slotIdx ) {
			string command = "LED D " + slotIdx;    //build the command
			ArduinoParser.Instance.SendCommand( command );
		}

		//timer for the chase event and send command
		private void SendChaseCommand() {
			string command = "LED C";
			ArduinoParser.Instance.SendCommand( command );
		}
	}
}