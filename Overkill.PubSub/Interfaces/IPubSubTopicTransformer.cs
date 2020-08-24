using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Overkill.PubSub.Interfaces
{
    public interface IPubSubTopicTransformer
    {
        Task<IPubSubTopic> Process(IPubSubTopic instruction);
    }
}
