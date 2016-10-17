using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSerial {

	public class Product : IEquatable<Product> {

		public enum Status { PutDown, PickedUp }

		public string name { get; private set; }

		public int productID { get; private set; }

        public int slotID { get; set; }

        public Status status { get; set; }

		public Product( string name, int productID, Status status = Status.PutDown ) {
			this.name = name;
			this.productID = productID;
			this.status = status;
		}

		public bool Equals( Product other ) {
			if ( other == null ) return false;
			return productID.Equals( other.productID );
		}

	}
}