using Overkill.Proxies.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace Overkill.Proxies
{
    /// <summary>
    /// Proxy class to assist in unit testing serial port communication functionality
    /// </summary>
    public class SerialProxy : ISerialProxy
    {
        private SerialPort serialPort;

        ISerialProxy Create(string device, int baudRate)
        {
            return new SerialProxy(device, baudRate);
        }

        public bool IsOpen { get { return serialPort.IsOpen; } }

        public SerialProxy() { }

        public SerialProxy(string device, int baudRate)
        {
            serialPort = new SerialPort(device, baudRate);
        }

        public void Close()
        {
            serialPort.Close();
        }

        public void Open()
        {
            serialPort.Open();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return serialPort.Read(buffer, offset, count);
        }

        public void Write(string data)
        {
            serialPort.Write(data);
        }
    }
}
