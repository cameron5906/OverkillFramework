using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Topics
{
    /// <summary>
    /// Used by Plugins to send customized websocket payloads to the driver on the Web platform. Usually this requires a plugin to also be enabled on the browser UI.
    /// </summary>
    public class PluginWebsocketMessageTopic : IPubSubTopic
    {
        public string MessageType { get; set; }
        public string JSON { get; set; }
    }
}
