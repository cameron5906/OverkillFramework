using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Topics
{
    /// <summary>
    /// Dictates the intent for a user to move the vehicle
    /// </summary>
    public class LocomotionTopic : IPubSubTopic
    {
        public float Direction { get; set; }
        public float Speed { get; set; }
    }
}
