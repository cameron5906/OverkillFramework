using Overkill.Websockets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Overkill.Websockets.Messages.Input
{
    /// <summary>
    /// Sent from a user interface when the user presses a keyboard key that has been bound, providing a pressed or released state.
    /// </summary>
    public class KeyboardInputMessage : IWebsocketMessage
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("pressed")]
        [Newtonsoft.Json.JsonProperty("pressed")]
        public bool IsPressed { get; set; }
    }
}
