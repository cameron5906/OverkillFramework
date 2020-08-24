using Overkill.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Connections.Data
{
    public class GpioData : ICommunicationPayload
    {
        public int Pin { get; set; }
        public int Status { get; set; }
    }
}
