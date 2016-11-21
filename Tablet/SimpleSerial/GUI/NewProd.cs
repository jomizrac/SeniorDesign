﻿using System;
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
        public int upc;
        public int slot;
        public NewProd(int upc, int slot)
        {
            this.upc = upc;
            this.slot = slot;
            InitializeComponent();
            label1.Text = "You have selected: " + upc + ", If this is correct, enter the new products UPC code";
        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int upc2 = Convert.ToInt32(textBox1.Text);
            NewProdConf confirm = new NewProdConf(upc2, slot);
            confirm.Show();
            this.Close();
        }
    }
}
