using Overkill.Core.Topics;
using Overkill.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vehicle.Traxxas.Topics;

namespace Vehicle.Traxxas.Middleware
{
    public class TraxxasInputTransformer : IPubSubTopicTransformer
    {
        public TraxxasInputTransformer(IServiceProvider serviceProvider) {}

        public async Task<IPubSubTopic> Process(IPubSubTopic topic)
        {
            var driveInstruction = (DriveInputTopic)topic;

            return await Task.Run(() => new TraxxasInputMessage()
            {
                ThrottleChannel = (byte)(driveInstruction.Throttle & 255),
                SteeringChannel = (byte)(driveInstruction.Steering & 255),
                BrakeChannel = 0
            });
        }
    }
}
