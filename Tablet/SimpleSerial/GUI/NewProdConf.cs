using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleSerial.GUI
{
    public partial class NewProdConf : Form
    {
        private int slot;
        private int upc;
        public NewProdConf(int upc, int slot)
        {
            this.slot = slot;
            this.upc = upc;
            InitializeComponent();
            label1.Text = Convert.ToString(upc);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShelfInventory.Instance.UpdateSlot(slot, upc);
            Menu menu = new Menu();
            menu.Show();
        }
    }
}
