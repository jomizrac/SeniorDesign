using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleSerial
{

    [Serializable]

    class VideoConfigs
    {
        public enum Behavior { Interruptable, Queued };
        public Behavior behavior = Behavior.Interruptable;

    }

}
