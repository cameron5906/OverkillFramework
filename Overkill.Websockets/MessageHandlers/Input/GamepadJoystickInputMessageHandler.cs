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
    public class GamepadJoystickInputMessageHandler : IWebsocketMessageHandler
    {
        private IPubSubService pubSub;

        public GamepadJoystickInputMessageHandler(IServiceProvider serviceProvider)
        {
            pubSub = serviceProvider.GetService<IPubSubService>();
        }

        public Task<IWebsocketMessage> Handle(IWebsocketMessage msg)
        {
            var joystickInput = (GamepadJoystickInputMessage)msg;

            pubSub.Dispatch(new GamepadJoystickInputTopic()
            {
                Name = joystickInput.Name,
                IsPressed = joystickInput.IsPressed,
                X = joystickInput.X,
                Y = joystickInput.Y
            });

            return null;
        }
    }
}
