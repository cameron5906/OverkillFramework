using Overkill.Websockets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Overkill.Websockets.Messages
{
    /// <summary>
    /// A generic Drive message
    /// Direction is 0-360
    /// </summary>
    public class LocomotionMessage : IWebsocketMessage
    {

        [JsonPropertyName("direction")]
        public float Direction { get; set; }
        [JsonPropertyName("speed")]
        public int Speed { get; set; }
    }
}
