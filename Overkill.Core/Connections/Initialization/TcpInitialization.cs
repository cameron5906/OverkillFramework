using Overkill.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Overkill.Core.Connections.Initialization
{
    public class TcpInitialization : IConnectionInitializer
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public IPEndPoint LocalEndpoint { get; set; }
    }
}
