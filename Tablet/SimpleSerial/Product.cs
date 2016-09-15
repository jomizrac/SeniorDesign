using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSerial {

    
	internal class Product {
        public string name { get; private set;  }
		public int productID { get; private set; } //The ID of the product (duh)
        public bool state { get; private set; } //false means product is in display, true means it is picked up
    }
}