using Overkill.Websockets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Overkill.Websockets.Messages.Input
{
    public class GamepadButtonInputMessage : IWebsocketMessage
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("pressed")]
        public bool IsPressed { get; set; }
    }
}
