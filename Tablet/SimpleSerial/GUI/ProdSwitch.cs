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
    public partial class ProdSwitch : Form
    {
        public ProdSwitch()
        {
            InitializeComponent();

            List<Product> localList = ShelfInventory.Instance.ProductList();
            List<Product> catalogList = Database.Instance.GetProductCatalog();

            ColumnHeader h1 = new ColumnHeader();
            ColumnHeader h2 = new ColumnHeader();
            ColumnHeader h3 = new ColumnHeader();
            h1.Text = "";
            h2.Text = "";
            h3.Text = "";
            h1.Name = "col1";
            h2.Name = "col2";
            h3.Name = "col3";
            h1.Width = completeList.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
            h2.Width = currentList.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
            h3.Width = slotList.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;

            completeList.Columns.Add(h1);
            currentList.Columns.Add(h2);
            slotList.Columns.Add(h3);

            foreach (var p in localList)
            {
                currentList.Items.Add("Slot: " + p.slotID + " " + p.name);
            }
            
            for(int i = 0; i < localList.Count; i++)
            {
                slotList.Items.Add(i.ToString());
            }

            foreach(var p in catalogList)
            {
                completeList.Items.Add(p.name);
            }

            
        }

        private void ProdSwitch_Load(object sender, EventArgs e)
        {

        }

        private void currentList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void completeList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
