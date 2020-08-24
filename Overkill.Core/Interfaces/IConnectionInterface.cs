using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Interfaces
{
    public interface IConnectionInterface
    {
        bool IsConnected { get; }
        void Initialize(IConnectionInitializer parameters);
        void Connect();
        void Send(ICommunicationPayload payload);
    }
}
