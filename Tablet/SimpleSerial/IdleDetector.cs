using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SimpleSerial {

	internal class IdleDetector {

		#region Singleton

		private static IdleDetector m_instance;

		public static IdleDetector Instance {
			get { return m_instance ?? ( m_instance = new IdleDetector() ); }
		}

		#endregion Singleton

		#region Events

		//not sure if i need this.
		public delegate void IdleProcess();

		public event IdleProcess IdleProcessEvent;

		#endregion Events

		//timer is set up for 5 minutes. time is in millis
		private long IDLE_TIMER = 300000;

		//intervals at which the stop watch is checked in millis
		private int WATCH_DELAY = 5000;

		//stop watch that is referenced against the IDLE_TIMER
		private Stopwatch stopwatch = new Stopwatch();

		private IdleDetector() {
			stopwatch.Start();

			new Thread( () => Initialize() ).Start();
			new Thread( () => CheckTime() ).Start();
		}

		private void Initialize() {
			ArduinoParser.Instance.SlotPickUpEvent -= Instance.ResetTimer;
			ArduinoParser.Instance.SlotPickUpEvent += Instance.ResetTimer;

			ArduinoParser.Instance.SlotPutDownEvent -= Instance.ResetTimer;
			ArduinoParser.Instance.SlotPutDownEvent += Instance.ResetTimer;
		}

		//happens when there is a pickup or putdown event. indicates someone
		//interacting with the shelf and the idle timer should be reset.
		//dont care about the slotIdx. it is just extra information
		private void ResetTimer( int slotIdx ) {
			stopwatch.Restart();
		}

		//checks the timer to see if a chase idle event needs to be sent
		private void CheckTime() {
			while ( true ) {
				if ( stopwatch.ElapsedMilliseconds >= IDLE_TIMER ) {
					IdleProcessEvent?.Invoke();
					stopwatch.Restart();
				}
				Thread.Sleep( WATCH_DELAY );
			}
		}
	}
}