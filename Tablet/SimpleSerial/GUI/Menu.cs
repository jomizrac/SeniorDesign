﻿using SimpleSerial.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleSerial
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            VideoInstr behavior = new VideoInstr();
            behavior.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SwapInstr swap = new SwapInstr();
            swap.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LogIn login = new LogIn();
            login.Show();
            this.Close();
        }
    }
}
