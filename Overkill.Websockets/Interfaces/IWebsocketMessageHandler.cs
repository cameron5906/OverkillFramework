using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Overkill.Websockets.Interfaces
{
    public interface IWebsocketMessageHandler
    {
        Task<IWebsocketMessage> Handle(IWebsocketMessage msg);
    }
}
