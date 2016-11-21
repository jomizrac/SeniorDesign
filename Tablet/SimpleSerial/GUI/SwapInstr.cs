using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SimpleSerial.GUI
{
    public partial class SwapInstr : Form
    {
        public string upc;
        public SwapInstr()
        {
            InitializeComponent();
            ShelfInventory.Instance.ProductPickUpEvent -= this.onProductPickup;
            ShelfInventory.Instance.ProductPickUpEvent += this.onProductPickup;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            this.Close();
        }
        private void onProductPickup( Product P )
        {
            upc = P.productID;
            NewProd newProd = new NewProd(upc, P.slotID);
            newProd.Show();
            this.Hide();
        }
    }
}
