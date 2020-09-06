using Overkill.Websockets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Overkill.Websockets.Messages.Input
{
    /// <summary>
    /// Sent from a user interface when the user clicks a button or some other triggering event, by registered name.
    /// </summary>
    public class GamepadTriggerInputMessage : IWebsocketMessage
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public float Value { get; set; }
    }
}
