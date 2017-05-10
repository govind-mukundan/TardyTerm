using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TardyTerm
{
    // Wraps a COM port with Pipes. For a single COM port, you can have at most 1 writer and N readers
    // In .NET pipe parlance, this translates to 1 PipeServerStream and N PipeClientStreams

    // win32 pipes - https://msdn.microsoft.com/en-us/library/windows/desktop/aa365150(v=vs.85).aspx
    class PipeCom
    {
        public SerialPortAdapter COMPortAdapter { get; set; }
        public List<NamedPipeClientStream> Readers { get; set;  }
        public List<NamedPipeServerStream> Writer { get; set; }
        public int BaudRate { get; set; }
        public string COMPortName { get; set; }
        public int Capacity { get; set; }
        public bool KeepWriting { get; set; }

        public PipeCom()
        {
            Capacity = 2048;
        }
        // Open the COM Port and setup named pipes, by default we have 1 writer a
        public bool Open(string port_name, int baud, int n_readers = 2)
        {
            try
            {
                COMPortName = port_name;
                COMPortAdapter = new SerialPortAdapter();
                if (COMPortAdapter.Open(port_name, baud))
                {
                    COMPortAdapter.SerialDataRxedHandler = RxByteStreamParser;
                    // Servers are necessary for all the traffic that comes in via the serial port
                    Writer = new List<NamedPipeServerStream>(n_readers);
                    Writer[0] = new NamedPipeServerStream(port_name + "in", PipeDirection.Out);
                    Writer[1] = new NamedPipeServerStream(port_name + "sniff", PipeDirection.InOut);

                    Readers = new List<NamedPipeClientStream>(1);
                    Readers[0] = new NamedPipeClientStream(port_name + "out"); // The server is provided by some other process
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
                return false;
        }

        public int RxByteStreamParser(byte[] bytes)
        {
            // All the data that comes in from the wire, goes into the server pipes

            //FIXME: use async-await to avoid blocking the callback
            // for wireshark you need to frame the packets before writing them into the sniff pipe: https://wiki.wireshark.org/CaptureSetup/Pipes
            for (int i=0; i<Writer.Count; i++)
            {
                if(Writer[i].IsConnected) // The write will block if no readers are connected
                    Writer[i].Write(bytes, 0, bytes.Length); 
            }
            return bytes.Length; // We Consume everything
        }

        // For writing data, we have a thread that will wait on the Reader Pipe
        public void ComWriteThread()
        {
            byte[] buffer = new byte[1];
            Readers[0].Connect();
            while (KeepWriting)
            {
                buffer[0] = (byte) Readers[0].ReadByte();
                COMPortAdapter.WriteData(buffer);

                // This data should also go to the sniffer
                // FIXME: Call the framing API here and write a frame
                // Probably best to use the "message" transmission mode for PCAP frames
            }
        }

    }
}
