using Microsoft.Extensions.DependencyInjection;
using Overkill.Core.Topics;
using Overkill.PubSub.Interfaces;
using Overkill.Websockets.Interfaces;
using Overkill.Websockets.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Overkill.Websockets.MessageHandlers
{
    /// <summary>
    /// Handler for generic Drive messages coming from users
    /// </summary>
    public class DriveMessageHandler : IWebsocketMessageHandler
    {
        IPubSubService pubSub;

        public DriveMessageHandler(IServiceProvider serviceProvider)
        {
            pubSub = serviceProvider.GetRequiredService<IPubSubService>();
        }

        public Task<IWebsocketMessage> Handle(IWebsocketMessage msg)
        {
            var drive = (DriveMessage)msg;

            pubSub.Dispatch(new DriveInputTopic()
            {
                Throttle = drive.Throttle,
                Steering = drive.Steering,
                Brake = drive.Brake
            });

            return null;
        }
    }
}
