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
    public partial class VideoInstr : Form
    {
        public VideoInstr()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            menu.BringToFront();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            VideoConf confirm = new GUI.VideoConf("queued");
            confirm.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            VideoConf confirm = new VideoConf("interrupt");
            confirm.Show();
            this.Close();
        }

        private void VideoInstr_Load(object sender, EventArgs e)
        {

        }
    }
}
