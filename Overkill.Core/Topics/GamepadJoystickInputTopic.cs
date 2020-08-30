using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Topics
{
    public class GamepadJoystickInputTopic : IPubSubTopic
    {
        public string Name { get; set; }
        public bool IsPressed { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }
}
