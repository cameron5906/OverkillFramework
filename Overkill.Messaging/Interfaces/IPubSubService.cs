using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Overkill.PubSub.Interfaces
{
    public interface IPubSubService
    {
        void DiscoverTopics();
        Task Dispatch(IPubSubTopic message);
        void Middleware<T>(Type middlewareType);
        void Transform<T>(Type transformerType);
        void Subscribe<T>(Action<T> listener);
    }
}
