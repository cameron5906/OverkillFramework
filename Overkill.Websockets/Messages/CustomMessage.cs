using Overkill.Websockets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Websockets.Messages
{
    /// <summary>
    /// A custom message payload coming from a plugin or other external process
    /// </summary>
    public class CustomMessage : IWebsocketMessage
    {
        public string MessageType { get; set; }
        public string JSON { get; set; }
    }
}
