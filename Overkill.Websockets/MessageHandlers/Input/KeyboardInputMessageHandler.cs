using Microsoft.Extensions.DependencyInjection;
using Overkill.Core.Topics;
using Overkill.Core.Topics.Input;
using Overkill.PubSub.Interfaces;
using Overkill.Websockets.Interfaces;
using Overkill.Websockets.Messages.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Overkill.Websockets.MessageHandlers.Input
{
    public class KeyboardInputMessageHandler : IWebsocketMessageHandler
    {
        private IPubSubService pubSub;

        public KeyboardInputMessageHandler(IServiceProvider serviceProvider)
        {
            pubSub = serviceProvider.GetService<IPubSubService>();
        }

        public Task<IWebsocketMessage> Handle(IWebsocketMessage msg)
        {
            var keyboardInput = (KeyboardInputMessage)msg;

            pubSub.Dispatch(new KeyboardInputTopic()
            {
                Name = keyboardInput.Name,
                IsPressed = keyboardInput.IsPressed
            });

            return null;
        }
    }
}
