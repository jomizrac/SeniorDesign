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

		public static string shelfInventoryFile = ConfigurationManager.AppSettings["shelfInventoryFile"];

		public Dictionary<int, Product> shelfSlots = new Dictionary<int, Product>();

		//private Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
		//		public List<Product> products = new List<Product>();
		public ShelfInventory() {
			Deserialize();

			//			new Thread( () => Initialize() ).Start();
		}

		public void UpdateSlot( int slotNumber, Product newProduct ) {
			// Update the list in memory
			shelfSlots.Add( slotNumber, newProduct );

			Serialize();

			// sync with cloud db
			//			DB.Instance.addShelf( products ); // TODO
		}

		public List<Product> ProductList() {
			return new List<Product>( shelfSlots.Values );
		}

		private void Initialize() {
			//			ArduinoParser.Instance.ProductPickUpEvent += Instance.OnProductPickup;
			//			ArduinoParser.Instance.ProductPutDownEvent += Instance.OnProductPutDown;
		}

		private void Serialize() {
			if ( !File.Exists( shelfInventoryFile ) ) {
				string directory = Path.GetDirectoryName( shelfInventoryFile );
				Directory.CreateDirectory( directory );
				File.Create( shelfInventoryFile );
			}

			File.WriteAllText( shelfInventoryFile, JsonConvert.SerializeObject( shelfSlots ) );
		}

		private void Deserialize() {
			if ( File.Exists( shelfInventoryFile ) ) {
				//				shelfSlots = JsonConvert.DeserializeObject<Dictionary<int, Product>>( File.ReadAllText( shelfInventoryFile ) );
				// TODO care, this is making the dictionary null some reason, may need some kind of try/catch
			}
		}
	}
}