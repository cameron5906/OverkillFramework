using Microsoft.Extensions.Configuration;
using Overkill.Core;
using Overkill.Core.Connections;
using Overkill.Core.Connections.Data;
using Overkill.Core.Connections.Initialization;
using Overkill.Core.Interfaces;
using Overkill.Core.Topics;
using Overkill.PubSub.Interfaces;
using Overkill.Services.Interfaces.Services;
using System;
using System.Net;
using Vehicle.Traxxas.Middleware;
using Vehicle.Traxxas.Topics;

namespace Vehicle.Traxxas
{
    /// <summary>
    /// Vehicle driver for Traxxas.
    /// This currently just transforms generic drive messages into TraxxasInputMessages and ends the relevent binary to the car's WiFi receiver.
    /// </summary>
    public class Traxxas : IVehicle
    {
        private IConnectionInterface _interface;
        private INetworkingService _networkingService;
        private IOverkillConfiguration _config;

        public Traxxas(IConnectionInterface connectionInterface, IPubSubService pubSub, INetworkingService networkingService, IOverkillConfiguration configuration)
        {
            _interface = connectionInterface;
            _networkingService = networkingService;
            _config = configuration;

            pubSub.Transform<DriveInputTopic>(typeof(TraxxasInputTransformer)); //Take the generic drive topic and transform it into a message Traxxas understands
            pubSub.Subscribe<TraxxasInputMessage>(HandleInput); //Subscribe to traxxas input messages
        }

        /// <summary>
        /// Connect to the vehicle via TCP protocol
        /// </summary>
        public void Initialize()
        {
            var localAddress = _networkingService.GetLocalInterfaceAddress(_config.VehicleConnection.Interface);

            _interface.Initialize(new TcpInitialization()
            {
                Host = _config.VehicleConnection.Host,
                Port = _config.VehicleConnection.Port,
                LocalEndpoint = new IPEndPoint(IPAddress.Parse(localAddress), 0)
            });
            _interface.Connect();

            Console.WriteLine($"Traxxas is ready");
        }

        /// <summary>
        /// Handler for TraxxasInputMessage. Builds a binary payload the WiFi receiver expects and sends it out.
        /// </summary>
        /// <param name="escInput">Input to send to the Traxxas ESC</param>
        public void HandleInput(TraxxasInputMessage escInput)
        {
            byte checksum = (byte)((85 + 0 + 11 + 0 + escInput.ThrottleChannel + escInput.SteeringChannel + escInput.BrakeChannel + 0 + 0 + 0) % 256);
            var packet = new byte[] { 85, 0, 11, 0, escInput.ThrottleChannel, escInput.SteeringChannel, escInput.BrakeChannel, 0, 0, 0, checksum };

            _interface.Send(new TcpData() { Data = packet });
        }
    }
}
