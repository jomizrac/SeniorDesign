using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SimpleSerial
{
    public partial class Form1 : Form
    {
        // Add this variable 
        string RxString;

        public Form1()
        {
            InitializeComponent();
            serialPort1.PortName = "COM4";
            serialPort1.BaudRate = 9600;
            // serialPort1
            serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);

            serialPort1.Open();
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void DisplayText(object sender, EventArgs e)
        {
            string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"data\");
            string file = path + RxString.Substring(0,1) + ".wmv";
            //MessageBox.Show(file);
            
            TimeSpan start = new TimeSpan(9, 0, 0); //9am
            TimeSpan end = new TimeSpan(18, 0, 0); //5pm
            TimeSpan now = DateTime.Now.TimeOfDay;

            if ((now > start) && (now < end))
            {
                axWindowsMediaPlayer1.Ctlcontrols.pause();
                axWindowsMediaPlayer1.URL = file;    //match found
            }
            
            // textBox1.AppendText(RxString);
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            RxString = serialPort1.ReadExisting();
            this.Invoke(new EventHandler(DisplayText));
        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {

        }
    }
}