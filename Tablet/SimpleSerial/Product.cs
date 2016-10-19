using System;

namespace SimpleSerial {

	public class Product : IEquatable<Product> {

		public enum Status { PutDown, PickedUp }

		public string name { get; private set; }

		public string productID { get; private set; }

		public int slotID { get; set; }

		public Status status { get; set; }

		public Product( string name, string productID, int slotID, Status status = Status.PutDown ) {
			this.name = name;
			this.productID = productID;
			this.slotID = slotID;
			this.status = status;
		}

		public bool Equals( Product other ) {
			if ( other == null ) return false;
			return productID.Equals( other.productID );
		}
	}
}