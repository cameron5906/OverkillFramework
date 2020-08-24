using Overkill.Websockets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Overkill.Websockets.Messages
{
    /// <summary>
    /// A generic Drive message
    /// </summary>
    public class DriveMessage : IWebsocketMessage
    {

        [JsonPropertyName("throttle")]
        public int Throttle { get; set; }
        [JsonPropertyName("steering")]
        public int Steering { get; set; }
        [JsonPropertyName("brake")]
        public bool Brake { get; set; }
    }
}
