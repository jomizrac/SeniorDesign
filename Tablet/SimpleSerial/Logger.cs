using System.Threading;

namespace SimpleSerial {

	public class Logger {

		#region Singleton

		private static Logger m_instance;

		public static Logger Instance {
			get { return m_instance ?? ( m_instance = new Logger() ); }
		}

		#endregion Singleton

		public Logger() {
			new Thread( () => Initialize() ).Start();
		}

		private void Initialize() {
			ShelfInventory.Instance.ProductPickUpEvent -= Instance.OnProductPickup;
			ShelfInventory.Instance.ProductPickUpEvent += Instance.OnProductPickup;

			ShelfInventory.Instance.ProductPutDownEvent -= Instance.OnProductPutDown;
			ShelfInventory.Instance.ProductPutDownEvent += Instance.OnProductPutDown;
		}

		private void OnProductPickup( Product product ) {
			Database.Instance.LogEvent( product, "Pick Up" );
		}

		private void OnProductPutDown( Product product ) {
			Database.Instance.LogEvent( product, "Put Down" );
		}
	}
}