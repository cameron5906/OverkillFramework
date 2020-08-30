using Overkill.Common.Enums;
using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Topics
{
    public class GamepadButtonInputTopic : IPubSubTopic
    {
        public string Name { get; set; }
        public InputState State { get; set; }
    }
}
