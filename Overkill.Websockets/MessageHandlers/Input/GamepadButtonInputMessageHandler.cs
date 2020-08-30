using Microsoft.Extensions.DependencyInjection;
using Overkill.Common.Enums;
using Overkill.Core.Topics;
using Overkill.PubSub.Interfaces;
using Overkill.Websockets.Interfaces;
using Overkill.Websockets.Messages.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Overkill.Websockets.MessageHandlers.Input
{
    public class GamepadButtonInputMessageHandler : IWebsocketMessageHandler
    {
        private IPubSubService pubSub;

        public GamepadButtonInputMessageHandler(IServiceProvider serviceProvider)
        {
            pubSub = serviceProvider.GetRequiredService<IPubSubService>();
        }

        public Task<IWebsocketMessage> Handle(IWebsocketMessage msg)
        {
            var gamepadButton = (GamepadButtonInputMessage)msg;

            pubSub.Dispatch(new GamepadButtonInputTopic()
            {
                Name = gamepadButton.Name,
                State = gamepadButton.IsPressed ? InputState.Pressed : InputState.Released
            });

            return null;
        }
    }
}
