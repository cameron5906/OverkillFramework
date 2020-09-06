using Overkill.Websockets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Overkill.Websockets.Messages.Input
{
    /// <summary>
    /// Sent from a user interface when they utilize a joystick to send relative X,Y coordinates with a bound input name
    /// </summary>
    public class GamepadJoystickInputMessage : IWebsocketMessage
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("pressed")]
        public bool IsPressed { get; set; }

        [JsonPropertyName("x")]
        public float X { get; set; }

        [JsonPropertyName("y")]
        public float Y { get; set; }
    }
}
