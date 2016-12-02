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
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            VideoManager.Instance.SetPlaybackMethod(VideoManager.PlaybackMethod.Immediate);
            Menu menu = new Menu();
            menu.Show();
            menu.BringToFront();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            VideoManager.Instance.SetPlaybackMethod(VideoManager.PlaybackMethod.Queued);
            Menu menu = new Menu();
            menu.Show();
            menu.BringToFront();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //set led chase to on

            Menu menu = new Menu();
            menu.Show();
            menu.BringToFront();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //set led chase to off

            Menu menu = new Menu();
            menu.Show();
            menu.BringToFront();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Menu menu = new Menu();
            menu.Show();
            menu.BringToFront();
            this.Close();
        }
    }
}
