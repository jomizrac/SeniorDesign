using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleSerial {

	internal class VideoManager {

		#region Singleton

		private static VideoManager m_instance;

		public static VideoManager Instance {
			get { return m_instance ?? ( m_instance = new VideoManager() ); }
		}

		#endregion Singleton
	}
}