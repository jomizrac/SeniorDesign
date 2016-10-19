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
        public VideoManager.PlaybackMethod behavior = VideoManager.PlaybackMethod.Immediate;

    }

}
