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

        //timer is set up for 5 minutes. time is in millis
        private long IDLE_TIMER = 300000;

        //stop watch that is referenced against the IDLE_TIMER
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
            String command = "LED U " + slotIdx;    //build the command
            ArduinoParser.Instance.SendCommand(command);

            stopwatch.Restart();
        }
        
        //deactivate light at a slot
        private void OnSlotPutDown(int slotIdx)
        {
            String command = "LED D " + slotIdx;    //build the command
            AdruinoParser.Instance.SendCommand(command);

            stopwatch.Restart();
        }

        //checks the timer to see if a chase effect needs to be send
        private void CheckTime()
        {
            while (true)
            {
                if(stopwatch.EllapsedMilliseconds >= IDLE_TIMER)
                {
                    SendChaseEvent();
                    stopwatch.Restart();
                }
                Thread.Sleep(5000);
            }
        }

        //timer for the chase event and send command
        private void SendChaseEvent()
        {
            String command = "LED C";
            ArduinoParser.Instance.SendCommand(command);
        }
    }
}