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
    public partial class NewProd : Form
    {
        public string upc;
        public int slot;
        public NewProd(string upc, int slot)
        {
            this.upc = upc;
            Console.WriteLine("upc: " + upc);
            this.slot = slot;
            Console.WriteLine("slot: " + slot);
            InitializeComponent();
            label1.Text = "You have selected: " + upc + ", If this is correct, enter the new products UPC code";
            Console.WriteLine("setting the label text");
            Console.WriteLine("label text: " + label1.Text);
        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string upc2 = textBox1.Text;
            NewProdConf confirm = new NewProdConf(upc2, slot);
            confirm.Show();
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void NewProd_Load(object sender, EventArgs e)
        {

        }
    }
}
