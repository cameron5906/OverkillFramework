using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Topics
{
    /// <summary>
    /// Informs any interested system in GPS location updates
    /// </summary>
    public class PositionUpdateTopic : IPubSubTopic
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
