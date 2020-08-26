using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Overkill.PubSub.Interfaces
{
    public interface IPubSubService
    {
        void DiscoverTopics();
        void Dispatch(IPubSubTopic message);
        void Middleware<T>(Func<T, T> function);
        void Transform<T>(Func<T, IPubSubTopic> function);
        void Subscribe<T>(Action<T> listener);
    }
}
