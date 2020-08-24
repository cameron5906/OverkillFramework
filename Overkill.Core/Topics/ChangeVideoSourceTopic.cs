using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Topics
{
    /// <summary>
    /// Dictates to the video transmission system to change which video device is being streamed
    /// </summary>
    public class ChangeVideoSourceTopic : IPubSubTopic
    {
        public string Device { get; set; }
    }
}
