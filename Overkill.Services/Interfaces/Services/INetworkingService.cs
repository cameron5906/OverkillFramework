using System.Net;
using System.Threading.Tasks;

namespace Overkill.Services.Interfaces.Services
{
    public interface INetworkingService
    {
        string GetLocalInterfaceAddress(string name);
        Task<string[]> GetNearbyNetworks();
    }
}
