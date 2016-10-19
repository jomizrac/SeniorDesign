using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

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
            new Thread(() => Initialize()).Start();

        }

        //register for events coming from arduino parser
        private void Initialize()
        {
            ArduinoParser.Instance.SlotPickUpEvent += Instance.OnSlotPickup;
            ArduinoParser.Instance.SlotPutDownEvent += Instance.OnSlotPutDown;
            IdleDetector.Instance.IdleProcessEvent += Instance.SendChaseCommand;
        }

        //activate light at a slot
        private void OnSlotPickup(int slotIdx)
        {
            String command = "LED U " + slotIdx;    //build the command
            ArduinoParser.Instance.SendCommand(command);
        }
        
        //deactivate light at a slot
        private void OnSlotPutDown(int slotIdx)
        {
            String command = "LED D " + slotIdx;    //build the command
            ArduinoParser.Instance.SendCommand(command);
        }

        //timer for the chase event and send command
        private void SendChaseCommand()
        {
            String command = "LED C";
            ArduinoParser.Instance.SendCommand(command);
        }
    }
}