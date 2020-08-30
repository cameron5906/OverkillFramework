using Overkill.Common.Configuration;
using Overkill.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Interfaces
{
    public interface IOverkillConfiguration
    {
        SystemConfiguration System { get; }
        ClientConfiguration Client { get; }
        PositioningConfiguration Positioning { get; }
        StreamingConfiguration Streaming { get; }
        VehicleConnectionConfiguration VehicleConnection { get; }
        InputConfiguration Input { get; }
    }
}
