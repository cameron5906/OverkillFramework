using Overkill.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Configuration
{
    public class VehicleConnectionConfiguration
    {
        public CommunicationProtocol Type { get; set; }
        public string Interface { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
