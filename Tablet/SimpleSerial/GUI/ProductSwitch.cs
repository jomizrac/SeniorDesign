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
    public partial class ProductSwitch : Form
    {
        public ProductSwitch()
        {
            InitializeComponent();

            List<Product> localList = ShelfInventory.Instance.ProductList();
            List<Product> catalogList = Database.Instance.GetProductCatalog();

            foreach (var p in localList)
            {
                currentList.Items.Add("Slot: " + p.slotID + " " + p.name);
            }

            completeList.Items.Add("None");
            foreach (var p in catalogList)
            {
                completeList.Items.Add(p.name);
            }
        }

        private void currentList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
