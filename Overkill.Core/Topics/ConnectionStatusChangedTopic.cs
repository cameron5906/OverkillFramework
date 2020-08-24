using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Topics
{
    /// <summary>
    /// Informs any interested system with the current connection state Overkill has to Overkill Web Services
    /// </summary>
    public class ConnectionStatusChangedTopic : IPubSubTopic
    {
        public bool Connected { get; set; }
    }
}
