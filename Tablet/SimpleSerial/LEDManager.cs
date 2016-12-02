using System.Threading;
using System;

namespace SimpleSerial {

	internal class LEDManager {

		#region Singleton

		private static object LOCK = new object();

		private static LEDManager m_instance;

		public static LEDManager Instance {
			get {
				lock ( LOCK ) {
					return m_instance ?? ( m_instance = new LEDManager() );
				}
			}
		}

        #endregion Singleton

        String LEDChase = "on";

		private LEDManager() {
			new Thread( () => Initialize() ).Start();
		}

		//register for events coming from arduino parser
		private void Initialize() {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            ArduinoParser.Instance.SlotPickUpEvent -= Instance.OnSlotPickup;
			ArduinoParser.Instance.SlotPickUpEvent += Instance.OnSlotPickup;

			ArduinoParser.Instance.SlotPutDownEvent -= Instance.OnSlotPutDown;
			ArduinoParser.Instance.SlotPutDownEvent += Instance.OnSlotPutDown;

			IdleDetector.Instance.IdleProcessEvent -= Instance.SendChaseCommand;
			IdleDetector.Instance.IdleProcessEvent += Instance.SendChaseCommand;
		}

		//activate light at a slot
		private void OnSlotPickup( int slotIdx ) {
			string command = "LED U " + slotIdx + "\n";    //build the command
			ArduinoParser.Instance.SendCommand( command );
		}

		//deactivate light at a slot
		private void OnSlotPutDown( int slotIdx ) {
			string command = "LED D " + slotIdx + "\n";    //build the command
			ArduinoParser.Instance.SendCommand( command );
		}

		//timer for the chase event and send command
		private void SendChaseCommand() {
            if (LEDChase.Equals("on"))
            {
                string command = "LED C\n";
                ArduinoParser.Instance.SendCommand(command);
            }else
            {
                //no chase
            }
		}

        public void TurnChaseOn()
        {
            LEDChase = "on";
        }

        public void TurnChaseOff()
        {
            LEDChase = "off";
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e) {
            string command = "LED O\n";
            ArduinoParser.Instance.SendCommand( command );
        }
    }
}