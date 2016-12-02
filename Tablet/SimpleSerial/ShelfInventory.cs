using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;

namespace SimpleSerial {

	/// <summary>
	/// Class responsibilities:
	/// Matches a slot index to a product
	///		Stores locally on disk and in cloud Database
	/// Translates slot index pickup/putdown events into product pickup/putdown events
	/// </summary>
	internal class ShelfInventory {

		#region Singleton

		private static object LOCK = new object();

		private static ShelfInventory m_instance;

		public static ShelfInventory Instance {
			get {
				lock ( LOCK ) {
					return m_instance ?? ( m_instance = new ShelfInventory() );
				}
			}
		}

		#endregion Singleton

		#region Events

		public delegate void ProductPickUp( Product product );

		public delegate void ProductPutDown( Product product );

		public delegate void SlotUpdated( int slotIdx, Product oldProduct, Product newProduct );

		public event ProductPickUp ProductPickUpEvent;

		public event ProductPutDown ProductPutDownEvent;

		public event SlotUpdated SlotUpdatedEvent;

		#endregion Events

		public static string shelfInventoryFile = ConfigurationManager.AppSettings["shelfInventoryFile"];

		private Dictionary<int, Product> slots = new Dictionary<int, Product>();

		private ShelfInventory() {
			Deserialize();
			new Thread( () => Initialize() ).Start();
		}

        public void UpdateSlot( int slotIdx, string UPC)
        {
            string name = Database.Instance.getProductName(UPC);
            Product newProduct = new Product(UPC, name, slotIdx);
            UpdateSlot(slotIdx, newProduct);
        }
		public void UpdateSlot( int slotIdx, Product newProduct ) {
			Product oldProduct = null;
			slots.TryGetValue( slotIdx, out oldProduct );

			if ( oldProduct == newProduct ) return;

			// Update the list in memory
			slots[slotIdx] = newProduct;
			newProduct.slotID = slotIdx;

			// Save the list to disk
			Serialize();

			// Sync with cloud db
			Database.Instance.UpdateShelfInventory( ProductList() );

			// Fire the event
			SlotUpdatedEvent?.Invoke( slotIdx, oldProduct, newProduct );
		}

		public List<Product> ProductList() {
			return new List<Product>( slots.Values );
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			foreach ( var prod in slots.Values ) {
				if ( sb.Length != 0 ) sb.Append( ";" );
				sb.Append( prod.productID );
			}
			sb.Insert( 0, "{" );
			sb.Append( "}" );
			return sb.ToString();
		}

		private void Initialize() {
			ArduinoParser.Instance.SlotPickUpEvent -= Instance.OnSlotPickup;
			ArduinoParser.Instance.SlotPickUpEvent += Instance.OnSlotPickup;

			ArduinoParser.Instance.SlotPutDownEvent -= Instance.OnSlotPutDown;
			ArduinoParser.Instance.SlotPutDownEvent += Instance.OnSlotPutDown;
		}

		/// <summary> Used to translate a slot pickup event into a product pickup event. </summary>
		private void OnSlotPickup( int slotIdx ) {
			if ( slots.ContainsKey( slotIdx ) ) {
				Product product = slots[slotIdx];
				product.status = Product.Status.PickedUp;
				ProductPickUpEvent?.Invoke( product );
			}
			else {
				Util.LogError( "Shelf inventory does not have a product assigned to slot: " + slotIdx );
			}
		}

		/// <summary> Used to translate a slot putdown event into a product putdown event. </summary>
		private void OnSlotPutDown( int slotIdx ) {
			if ( slots.ContainsKey( slotIdx ) ) {
				Product product = slots[slotIdx];
				product.status = Product.Status.PutDown;
				ProductPutDownEvent?.Invoke( product );
			}
			else {
				Util.LogError( "Shelf inventory does not have a product assigned to slot: " + slotIdx );
			}
		}

		private void Serialize() {
			if ( !File.Exists( shelfInventoryFile ) ) {
				string directory = Path.GetDirectoryName( shelfInventoryFile );
				Directory.CreateDirectory( directory );
			}

			File.WriteAllText( shelfInventoryFile, JsonConvert.SerializeObject( slots ) );
			Util.LogSuccess( "Saved shelf inventory: " + this );
		}

		private void Deserialize() {
			if ( File.Exists( shelfInventoryFile ) ) {
				slots = JsonConvert.DeserializeObject<Dictionary<int, Product>>( File.ReadAllText( shelfInventoryFile ) );
				Util.LogSuccess( "Loaded shelf inventory: " + this );
			}
			else {
				Util.LogWarning( "No local shelf inventory file found" );
				slots = new Dictionary<int, Product>();
			}
		}
	}
}