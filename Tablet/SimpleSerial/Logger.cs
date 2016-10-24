using System.IO;
using System.Threading;

namespace SimpleSerial {
	public class Logger {
		#region Singleton

		private static Logger m_instance;

		public static Logger Instance {
			get { return m_instance ?? ( m_instance = new Logger() ); }
		}

		#endregion Singleton

		private TextWriter log;

		private Logger() {
			new Thread( () => Initialize() ).Start();
		}

		private void Initialize() {
		}

		public void Log( string message ) {
		}

		private void DeleteOldLogs() {
		}
	}
}