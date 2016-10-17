using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSerial {

	internal class LEDManager {

		#region Singleton

		private static LEDManager m_instance;

		public static LEDManager Instance {
			get { return m_instance ?? ( m_instance = new LEDManager() ); }
		}

		#endregion Singleton


        private LEDManager()
        {
            Initializer();
        }

        private void Initialize()
        {
            ArduinoParser.Instance.SlotPickUpEvent += Instance.OnSlotPickup;
            ArduinoParser.Instance.SlotPutDownEvent += Instance.OnSlotPutDown;
        }
        

        //register for events coming from arduino parser

        //activate light at a slot
        private void OnSlotPickup(int slot)
        {
            String command = "";    //build the command
            ArduinoParser.Instance.SendCommand(command);
        }
        
        //deactivate light at a slot
        private void OnSlotPickup(int slot)
        {
            String command = "";    //build the command
            AdruinoParser.Instance.SendCommand(command);
        }
        //activate the chase effect
    }
}