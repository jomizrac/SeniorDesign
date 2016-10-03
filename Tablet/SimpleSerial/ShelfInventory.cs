using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace SimpleSerial {

	internal class ShelfInventory {

		#region Singleton

		private static ShelfInventory m_instance;

		public static ShelfInventory Instance {
			get { return m_instance ?? ( m_instance = new ShelfInventory() ); }
		}

		#endregion Singleton

		//private Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
		public List<Product> products = new List<Product>();

		public List<Product> playables = new List<Product>();

		public ShelfInventory() {
			if ( File.Exists( ConfigurationManager.AppSettings["shelfInventoryFile"] ) ) {
				//				products = JsonConvert.DeserializeObject<List<Product>>( File.ReadAllText( ConfigurationManager.AppSettings["shelfInventoryFile"] ) );
			}

			//			ArduinoParser.Instance.ProductPickUpEvent += Instance.OnProductPickup;
			//			ArduinoParser.Instance.ProductPutDownEvent += Instance.OnProductPutDown;
		}

		public void UpdateSlot( int slotNumber, Product newProduct ) {
			// Update the list
			products.Capacity = slotNumber >= products.Count ? slotNumber + 1 : products.Count;
			products[slotNumber] = newProduct;

			// Serialize the current list to disk
			File.WriteAllText( ConfigurationManager.AppSettings["shelfInventoryFile"], JsonConvert.SerializeObject( products ) );

			// sync with cloud db
			//			DB.Instance.addShelf( products ); // TODO
		}

		private void OnProductPickup( int slotID ) {
			//			Product current = new Product();
			//			current = products[slotID];

			//			current.state = true;
			//			playables.Add( current );
		}

		private void OnProductPutDown( int slotID ) {
			//			Product current = new Product();
			//			current = products[slotID];

			//			current.state = true;
			//			playables.Remove( current );
		}
	}
}