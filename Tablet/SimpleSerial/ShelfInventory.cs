using System;
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

		public List<Product> products = new List<Product>();

		public void UpdateSlot( int slotNumber, Product newProduct ) {
			products[slotNumber] = newProduct;
		}
	}
}