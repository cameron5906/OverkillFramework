using Microsoft.Extensions.DependencyInjection;
using Overkill.Core.Topics;
using Overkill.PubSub.Interfaces;
using Overkill.Websockets.Interfaces;
using Overkill.Websockets.Messages;
using Overkill.Websockets.Messages.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Overkill.Websockets.MessageHandlers.Input
{
    public class GamepadTriggerInputMessageHandler : IWebsocketMessageHandler
    {
        private IPubSubService pubSub;

        public GamepadTriggerInputMessageHandler(IServiceProvider serviceProvider)
        {
            pubSub = serviceProvider.GetService<IPubSubService>();
        }

        public Task<IWebsocketMessage> Handle(IWebsocketMessage msg)
        {
            var triggerInput = (GamepadTriggerInputMessage)msg;

            pubSub.Dispatch(new GamepadTriggerInputTopic()
            {
                Name = triggerInput.Name,
                Value = triggerInput.Value
            });

            return null;
        }
    }
}
