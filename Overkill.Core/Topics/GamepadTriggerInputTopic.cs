using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Topics
{
    public class GamepadTriggerInputTopic : IPubSubTopic
    {
        public string Name { get; set; }
        public float Value { get; set; }
    }
}
