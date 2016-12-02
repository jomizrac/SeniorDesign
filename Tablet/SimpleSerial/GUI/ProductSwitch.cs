using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleSerial.GUI {

	public partial class ProductSwitch : Form {

		public ProductSwitch() {
			InitializeComponent();

			List<Product> localList = ShelfInventory.Instance.ProductList();
			List<Product> catalogList = Database.Instance.GetProductCatalog();

			foreach ( var p in localList ) {
				currentList.Items.Add( "Slot: " + p.slotID + " " + p.name);
			}

			//completeList.Items.Add( "None" );
			foreach ( var p in catalogList ) {
				completeList.Items.Add(p.productID + " " + p.name);
			}
		}

		private void currentList_SelectedIndexChanged( object sender, EventArgs e ) {
		}

		private void completeList_SelectedIndexChanged( object sender, EventArgs e ) {
		}

		private void button2_Click( object sender, EventArgs e ) {
			Menu menu = new Menu();
			menu.Show();
			menu.BringToFront();
			this.Close();
		}

		private void button1_Click( object sender, EventArgs e ) {
            //String pCurrent = currentList.SelectedItem.ToString();
            //String pNew = completeList.SelectedItem.ToString();
            button1.Text = "Current List Updated";
            
            //ShelfInventory.Instance.UpdateSlot(Int32.Parse(pCurrent.Substring(6, 1)), pNew.Substring(0,11));
            //currentList.Refresh();
            
		}

		private void ProductSwitch_Load( object sender, EventArgs e ) {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        }

		private void ProductSwitch_Deactivate( object sender, EventArgs e ) {
			this.Close();
		}

        private void backButton_Click(object sender, EventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            menu.BringToFront();
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            String pCurrent = currentList.SelectedItem.ToString();
            String pNew = completeList.SelectedItem.ToString();

            ShelfInventory.Instance.UpdateSlot(Int32.Parse(pCurrent.Substring(6, 1)), pNew.Substring(0,12));
            currentList.Refresh();
        }
    }
}
