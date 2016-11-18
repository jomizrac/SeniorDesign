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
        public int upc;
        public NewProd()
        {
            InitializeComponent();
        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            upc = Convert.ToInt32(textBox1.Text);
            NewProdConf confirm = new NewProdConf();
            confirm.Show();
            this.Close();
        }
    }
}
