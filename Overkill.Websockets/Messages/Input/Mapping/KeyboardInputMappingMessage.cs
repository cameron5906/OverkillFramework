using Overkill.Websockets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Overkill.Websockets.Messages.Input.Mapping
{
    public class KeyboardInputMappingMessage : IWebsocketMessage
    {
        [JsonPropertyName("mapping")]
        public Dictionary<string, int> Mapping { get; set; }
    }
}
