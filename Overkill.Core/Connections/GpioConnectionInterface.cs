using Overkill.Core.Connections.Data;
using Overkill.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Connections
{
    /// <summary>
    /// GPIO communication service for receiving/transmitting data from/to Vehicles using a direct pin interface
    /// </summary>
    public class GpioConnectionInterface : IConnectionInterface
    {
        public bool IsConnected { get; set; }

        public void Initialize(IConnectionInitializer parameters)
        {

        }

        public void Connect()
        {

        }

        public void Send(ICommunicationPayload payload)
        {
            var gpio = (GpioData)payload;

        }
    }
}
