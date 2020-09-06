using Microsoft.Extensions.Configuration;
using Overkill.Common.Configuration;
using Overkill.Core.Configuration;
using Overkill.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core
{
    public class OverkillConfiguration : IOverkillConfiguration
    {
        public SystemConfiguration System { get; set; }
        public ClientConfiguration Client { get; set; }
        public PositioningConfiguration Positioning { get; set; }
        public VehicleConnectionConfiguration VehicleConnection { get; set; }
        public StreamingConfiguration Streaming { get; set; }
        public InputConfiguration Input { get; set; }

        public OverkillConfiguration() { }
        public OverkillConfiguration(IOverkillConfiguration config)
        {
            System = config.System;
            Client = config.Client;
            Positioning = config.Positioning;
            VehicleConnection = config.VehicleConnection;
            Streaming = config.Streaming;
            Input = config.Input;
        }
    }
}
