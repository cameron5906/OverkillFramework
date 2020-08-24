using Microsoft.Extensions.Configuration;
using Overkill.Proxies.Interfaces;
using Overkill.Services.Interfaces;
using Overkill.Services.Interfaces.Services;
using Overkill.Util.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Overkill.Services
{
    /// <summary>
    /// Responsible for networking related functionality
    /// TODO: Make the code in here less platform dependent
    /// </summary>
    public class NetworkingService : INetworkingService
    {
        private IProcessProxy processProxy;

        public NetworkingService(IProcessProxy _processProxy)
        {
            processProxy = _processProxy;
        }

        public string GetLocalInterfaceAddress(string name)
        {
            var networkInterface = NetworkInterface
                        .GetAllNetworkInterfaces()
                        .FirstOrDefault(inet =>
                            inet.Name == name
                        );
            if (networkInterface == null) throw new Exception("Failed to find network interface");

            var addressInfo = networkInterface
                                    .GetIPProperties()
                                    .UnicastAddresses
                                        .FirstOrDefault(addr =>
                                            addr.Address.AddressFamily == AddressFamily.InterNetwork
                                        );
            return addressInfo.Address.MapToIPv4().ToString();
        }

        public async Task<string[]> GetNearbyNetworks()
        {
            var (exitCode, output, errorOutput) = await processProxy.ExecuteShellCommand("iw", "dev", "wlan1", "scan");

            if (exitCode != 0)
                throw new Exception("iw command failed in GetNearbyNetworks: " + errorOutput);

            var lines = output.Split(Environment.NewLine);

            var networkNames = lines
                                    .Where(line => line.Contains("SSID:"))
                                    .Select(line => line.Split(new[] { "SSID:" }, StringSplitOptions.None)[1])
                                    .Where(ssid => ssid.Length < 20)
                                    .Distinct()
                                    .ToArray();

            return networkNames;
        }
    }
}
