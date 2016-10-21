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

		public static bool operator ==( Product a, Product b ) {
			// If both are null, or both are same instance, return true.
			if ( ReferenceEquals( a, b ) ) {
				return true;
			}

			// If one is null, but not both, return false.
			if ( a == null || b == null ) {
				return false;
			}

			// Return true if the fields match:
			return a.Equals( b );
		}

		public static bool operator !=( Product a, Product b ) {
			return !( a == b );
		}

		public bool Equals( Product other ) {
			if ( other == null ) return false;
			return productID.Equals( other.productID );
		}
	}
}