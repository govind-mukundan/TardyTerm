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
using System.Timers;

namespace TardyTerm
{
    public partial class Form1 : Form
    {

        int BAUD_RATE = 230400;
        bool _logging = false;
        bool _portOpened = false;
        string DEFAULT_FILE = "Tardy.log";
        BinaryWriter BW;
        HRAnalyzer HRA;
        SerialPortAdapter _comInterface;

        // Timer to update the HR
        System.Timers.Timer UIUpdateTimer;
        int C_UI_UPDATE_INTERVAL = 100;

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
            HRA = new HRAnalyzer();
            UIUpdateTimer = new System.Timers.Timer(C_UI_UPDATE_INTERVAL);
            UIUpdateTimer.Elapsed += new ElapsedEventHandler(OnTimeElapsed);
            
        }

        void OnTimeElapsed(object source, ElapsedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lbl_HR.Text = "HR: " + HRA.HR.ToString();
            });
            
        }

        private void btn_OpenJigPort_Click(object sender, EventArgs e)
        {
            if (_portOpened == false)
            {
                _comInterface = new SerialPortAdapter();
                if (Open(cbCOMPorts.Text))
                {
                    _comInterface.SerialDataRxedHandler = RxByteStreamParser;

                    btn_OpenJigPort.Text = "Disconnect!";
                    _portOpened = true;

                    UIUpdateTimer.Enabled = true;
                    UIUpdateTimer.Start();
                }
            }
            else if (_portOpened == true)
            {
                _comInterface.Close();
                btn_OpenJigPort.Text = "Connect!";
                _portOpened = false;

                UIUpdateTimer.Enabled = false;
                UIUpdateTimer.Stop();
            }
        }

        public bool Open(string comPort)
        {
            Debug.WriteLine("Opening port to:" + comPort);
            return (_comInterface.Open(comPort, BAUD_RATE));
        }

        public int RxByteStreamParser(byte[] bytes)
        {
            int len = ((bytes.Length >> 1) << 1);
            for (int i = 0; i < len; i = i+2)
            {
                //Debug.Write(Convert.ToString(bytes[i]) + " ");
                HRA.Analyze(bytes[i] | ((UInt16)bytes[i + 1] << 8));

                if (BW != null) BW.Write((UInt16)(bytes[i] | ((UInt16)bytes[i + 1] << 8)));
            }

            return len;
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
