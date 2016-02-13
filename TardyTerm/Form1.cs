using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TardyTerm
{
    public partial class Form1 : Form
    {

        int BAUD_RATE = 230400;
        bool _logging = false;
        bool _portOpened = false;
        string DEFAULT_FILE = "Tardy.log";
        BinaryWriter BW;

        public Form1()
        {
            InitializeComponent();

            foreach (string s in SerialPort.GetPortNames())
            {
                cbCOMPorts.Items.Add(s);
            }
            if (cbCOMPorts.Items.Count > 0)
            {
                cbCOMPorts.SelectedIndex = 0;
            }
        }

        SerialPortAdapter _comInterface;
        private void btn_OpenJigPort_Click(object sender, EventArgs e)
        {
            if (_portOpened == false && Open(cbCOMPorts.Text))
            {
                _comInterface = new SerialPortAdapter();
                _comInterface.SerialDataRxedHandler = RxByteStreamParser;

                btn_OpenJigPort.Text = "Disconnect!";
                _portOpened = true;
            }
            else if (_portOpened == true)
            {
                _comInterface.Close();
                btn_OpenJigPort.Text = "Connect!";
                _portOpened = false;
            }
        }

        public bool Open(string comPort)
        {
            Debug.WriteLine("Opening port to:" + comPort);
            return (_comInterface.Open(comPort, BAUD_RATE));
        }

        public int RxByteStreamParser(byte[] bytes)
        {
            if (BW != null) BW.Write(bytes);
            for(int i=0; i<bytes.Length; i++)
                Debug.Write(Convert.ToString(bytes[i]) + " ");

            return bytes.Length;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_logging) _logging = false;
            else _logging = true;

            if (_logging)
            {
                // Open a binary file in the given path. If path = invalid, use default file name
                if(txtBx_LogFileName.Text == "")
                    BW = new BinaryWriter(File.Open(DEFAULT_FILE, FileMode.Create));
                else
                    BW = new BinaryWriter(File.Open(txtBx_LogFileName.Text, FileMode.Create));

                btn_Log.Text = "Stop Logging";
            }
            else
            {
                btn_Log.Text = "Start Logging";
                if(BW != null) BW.Close();
                BW = null;

            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (BW != null) BW.Close();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
