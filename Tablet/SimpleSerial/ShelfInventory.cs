using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

		public static string jsonFile = @"C:\ShelfRokr\Data\products.json";

		//private Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
		public List<Product> products = new List<Product>();

		private List<Product> playables = new List<Product>();

		public ShelfInventory() {
			if ( !File.Exists( jsonFile ) ) {
				products = JsonConvert.DeserializeObject<List<Product>>( File.ReadAllText( jsonFile ) );
			}
		}

		public void UpdateSlot( int slotNumber, Product newProduct ) {
			// Update the list
			products[slotNumber] = newProduct;

			// Serialize the current list to disk
			File.WriteAllText( jsonFile, JsonConvert.SerializeObject( products ) );

            // sync with cloud db
            DB.Instance.addShelf(products);
			// TODO
		}

		private static void Main() {
			ArduinoParser.Instance.ProductPickUpEvent += Instance.OnProductPickup;
			ArduinoParser.Instance.ProductPutDownEvent += Instance.OnProductPutDown;
		}

		private void OnProductPickup( int slotID ) {
			Product current = new Product();
			current = products[slotID];

			current.state = true;
			playables.Add( current );
		}

		private void OnProductPutDown( int ID ) {
			Product current = new Product();
			current = products[slotID];

			current.state = true;
			playables.Remove( current );
		}
	}
}