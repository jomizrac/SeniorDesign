﻿using System;
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

        private StopWatch stopwatch;

        private LEDManager()
        {
            Initialize();
            stopwatch = new StopWatch();
            stopwatch.Start();

            new Thread(() => CheckTime()).Start();
        }

        //register for events coming from arduino parser
        private void Initialize()
        {
            ArduinoParser.Instance.SlotPickUpEvent += Instance.OnSlotPickup;
            ArduinoParser.Instance.SlotPutDownEvent += Instance.OnSlotPutDown;
        }

        //activate light at a slot
        private void OnSlotPickup(int slotIdx)
        {
            String command = slotIdx + " U" ;    //build the command
            ArduinoParser.Instance.SendCommand(command);

            stopwatch.Restart();
        }
        
        //deactivate light at a slot
        private void OnSlotPutDown(int slotIdx)
        {
            String command = slotIdx + " D";    //build the command
            AdruinoParser.Instance.SendCommand(command);

            stopwatch.Restart();
        }

        private void CheckTime()
        {
            while (true)
            {

            }
        }

        //timer for the chase event and send command
        private void SendChaseEvent()
        {
            String command = "CS";
            ArduinoParser.Instance.SendCommand(command);
        }
    }
}