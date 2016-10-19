using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSerial
{
   internal class IdleDetector
    {
        #region Singleton

        private static ArduinoParser m_instance;

        public static ArduinoParser Instance
        {
            get { return m_instance ?? (m_instance = new ArduinoParser()); }
        }

        #endregion Singleton

        #region Events

        public delegate void SlotPickUp(int slotIdx);

        public delegate void SlotPutDown(int slotIdx);

        public event SlotPickUp SlotPickUpEvent;

        public event SlotPutDown SlotPutDownEvent;

        #endregion Events
    }
}
