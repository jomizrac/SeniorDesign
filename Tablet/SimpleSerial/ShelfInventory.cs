﻿using System;
using System.Collections.Generic;
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

		private List<Product> playables = new List<Product>();

		public void UpdateSlot( int slotNumber, Product newProduct ) {
			products[slotNumber] = newProduct;
			// store locally somehow
			// sync with cloud db
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