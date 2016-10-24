using System.Threading;

namespace SimpleSerial {

	public class DatabaseLogger {

		#region Singleton

		private static DatabaseLogger m_instance;

		public static DatabaseLogger Instance {
			get { return m_instance ?? ( m_instance = new DatabaseLogger() ); }
		}

		#endregion Singleton

		private DatabaseLogger() {
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