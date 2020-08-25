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
    public class LocomotionMessageHandler : IWebsocketMessageHandler
    {
        IPubSubService pubSub;

        public LocomotionMessageHandler(IServiceProvider serviceProvider)
        {
            pubSub = serviceProvider.GetRequiredService<IPubSubService>();
        }

        public Task<IWebsocketMessage> Handle(IWebsocketMessage msg)
        {
            var locomotion = (LocomotionMessage)msg;

            pubSub.Dispatch(new LocomotionTopic()
            {
                Direction = locomotion.Direction,
                Speed = locomotion.Speed
            });

            return null;
        }
    }
}
