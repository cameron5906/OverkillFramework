using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Proxies.Interfaces
{
    public interface ISerialProxy
    {
        ISerialProxy Create(string device, int baudRate) { return null; }
        bool IsOpen { get; }
        void Open();
        void Write(string data);
        int Read(byte[] buffer, int offset, int count);
        void Close();
    }
}
