using Overkill.Websockets.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Websockets.Messages
{
    /// <summary>
    /// Used to authenticate the vehicle with online services
    /// </summary>
    public class VehicleAuthorizationMessage : IWebsocketMessage
    {
        public string Token { get; set; }
    }
}
