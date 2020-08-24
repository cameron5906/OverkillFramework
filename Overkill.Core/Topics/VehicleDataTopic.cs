using Overkill.Core.Interfaces;
using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Topics
{
    /// <summary>
    /// Sends data coming back from the Vehicle to any interested system
    /// </summary>
    public class VehicleDataTopic : IPubSubTopic
    {
        public ICommunicationPayload Payload { get; set; }
    }
}
