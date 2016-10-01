﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSerial {

	public class Product {

		public enum Status { PutDown, PickedUp }

		public string name { get; private set; }

		public int productID { get; private set; }

		public Status status { get; private set; }

		public Product( string name, int productID, Status status = Status.PutDown ) {
			this.name = name;
			this.productID = productID;
			this.status = status;
		}
	}
}