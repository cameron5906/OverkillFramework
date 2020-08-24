using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Overkill.Core.Interfaces;
using Overkill.Core.Topics;
using Overkill.PubSub.Interfaces;
using Overkill.Services.Interfaces.Services;
using Overkill.Websockets.Interfaces;
using Overkill.Websockets.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp.NetCore;

namespace Overkill.Websockets
{
    /// <summary>
    /// Manages the Websocket client communication between Overkill and Overkill Web Services
    /// This service utilizes reflection to discover messages and their respective handler functions.
    /// Check out the README to see how easy it is to add new message types
    /// </summary>
    public class WebsocketService : IWebsocketService
    {
        private IServiceProvider _serviceProvider;
        private INetworkingService _networkService;
        private IOverkillConfiguration _config;
        private Dictionary<string, IWebsocketMessageHandler> messageHandlerCache;
        private Dictionary<string, Type> messageTypeCache;
        private WebSocket webSocket;

        public WebsocketService(IServiceProvider serviceProvider, INetworkingService networkService, IOverkillConfiguration configuration, IPubSubService pubSub)
        {
            _serviceProvider = serviceProvider;
            _networkService = networkService;
            _config = configuration;

            messageHandlerCache = new Dictionary<string, IWebsocketMessageHandler>();
            messageTypeCache = new Dictionary<string, Type>();

            pubSub.Subscribe<PluginWebsocketMessageTopic>(message =>
            {
                SendMessage(new CustomMessage() {
                    MessageType = message.MessageType,
                    JSON = message.JSON
                });
            });
        }

        /// <summary>
        /// Start up the client by registering message types, handlers, and then connecting
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            RegisterMessages();
            RegisterMessageHandlers();
            await Connect();
        }

        /// <summary>
        /// Configure the websocket and assign event handlers, connect using system configuration
        /// </summary>
        /// <returns></returns>
        public async Task Connect()
        {
            var localIP = IPAddress.Parse(_networkService.GetLocalInterfaceAddress(_config.Client.Interface));
            webSocket = new WebSocket(new IPEndPoint(localIP, 0), _config.Client.ConnectionString);
            webSocket.Log.Level = LogLevel.Debug;

            webSocket.WaitTime = TimeSpan.FromSeconds(1);
            webSocket.OnMessage += async (sender, evt) => await Handle(evt.Data);
            webSocket.OnOpen += (sender, evt) => OnConnected();
            webSocket.OnError += (sender, evt) => Console.WriteLine(evt.Message);
            webSocket.OnClose += (sender, evt) => OnDisconnected();
            await webSocket.ConnectAsync();
        }

        /// <summary>
        /// Event handler for a successful connection to online services. Sends an authentication message.
        /// </summary>
        private void OnConnected()
        {
            if(webSocket.IsAlive)
            {
                Console.WriteLine("Authenticating with Web Services...");
                SendMessage(new VehicleAuthorizationMessage()
                {
                    Token = _config.System.AuthorizationToken
                });
            }
        }

        /// <summary>
        /// Event handler that fires off when connection is lost with web services.
        /// TODO: Dispatch kill switch?
        /// </summary>
        private void OnDisconnected()
        {
            Console.WriteLine("Lost connection with web services. Reconnecting...");

            //Wait a second and re-connect
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                webSocket.ConnectAsync();
            });
        }

        /// <summary>
        /// Function to parse an incoming message, check if its a known message type, and send it off to its respective handler
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task Handle(string json)
        {
            //Parse the message type
            var message = JObject.Parse(json);
            var messageType = (string)message["type"];

            //Check to see if we have a Type cached for this
            var messageClassType = messageTypeCache[messageType];

            //If not, return. Invalid message.
            if (!messageHandlerCache.ContainsKey(messageType)) return;
            
            //Otherwise, deserialize the JSON into this Type
            var convertedMessage = message.ToObject(messageClassType);

            //Retrieve the IWebsocketMessageHandler and invoke it
            var handler = messageHandlerCache[messageType];
            var task = handler.Handle((IWebsocketMessage)convertedMessage);
            
            //If there is no response, return. Otherwise, send the response.
            if (task == null) return;
            SendMessage(await task);
        }

        /// <summary>
        /// Use reflection to discover message handlers in the loaded assemblies by searching for IWebsocketMessageHandler inheritance.
        /// Additionally, ensure these classes obey the naming convention of having "MessageHandler" at the end of their name.
        /// Cache them in a dictionary for future lookups.
        /// </summary>
        public void RegisterMessageHandlers()
        {
            var handlerInstances = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IWebsocketMessageHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract && !x.IsGenericType)
                .Where(x => x.Name.ToLower().EndsWith("messagehandler"))
                .Select(x => Activator.CreateInstance(x, new[] { _serviceProvider }) as IWebsocketMessageHandler)
                .ToList();

            Console.WriteLine($"{handlerInstances.Count} websocket message handlers found");

            handlerInstances.ForEach(instance =>
            {
                var messageType = instance.GetType().Name.ToLower().Split(new[] { "messagehandler" }, StringSplitOptions.None)[0];

                messageHandlerCache.Add(
                    messageType,
                    instance
                );

                Console.WriteLine($"Registered websocket message handler for type {messageType}");
            });
        }

        /// <summary>
        /// Use reflection to discover message types in loaded assemblies by searchign for IWebsocketMessage inheritance.
        /// Additionally, ensure these classes obey the naming conventions of having "Message" at the end of their name.
        /// Cache them in a dictionary for future lookups.
        /// </summary>
        public void RegisterMessages()
        {
            var messageTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IWebsocketMessage).IsAssignableFrom(x) && !x.IsInterface)
                .Where(x => x.Name.ToLower().EndsWith("message"))
                .ToList();

            Console.WriteLine($"{messageTypes.Count} websocket message types found");

            messageTypes.ForEach(type =>
            {
                var messageType = type.Name.ToLower().Split(new[] { "message" }, StringSplitOptions.None)[0];

                messageTypeCache.Add(
                    messageType,
                    type
                );

                Console.WriteLine($"Registered websocket message: {messageType}");
            });
        }

        /// <summary>
        /// Send a message to the server.
        /// A "type" property will automatically be assigned based on the Type name.
        /// </summary>
        /// <param name="message">A message inheriting the IWebsocketMessage interface</param>
        public void SendMessage(IWebsocketMessage message)
        {
            if (!messageTypeCache.Any(x => x.Value == message.GetType())) return;

            var messageType = messageTypeCache.FirstOrDefault(x => x.Value == message.GetType()).Key;
            var jObject = JObject.FromObject(message);
            jObject.Add("type", messageType);
            webSocket.Send(jObject.ToString());
        }
    }
}
