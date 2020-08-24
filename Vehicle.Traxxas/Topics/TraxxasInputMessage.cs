using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Vehicle.Traxxas.Topics
{
    public class TraxxasInputMessage : IPubSubTopic
    {
        public byte ThrottleChannel { get; set; }
        public byte SteeringChannel { get; set; }
        public byte BrakeChannel { get; set; }
    }
}
