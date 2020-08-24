using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Topics
{
    /// <summary>
    /// Dictates the intent for a user to move the vehicle
    /// </summary>
    public class DriveInputTopic : IPubSubTopic
    {
        public int Throttle { get; set; }
        public int Steering { get; set; }
        public bool Brake { get; set; }
    }
}
