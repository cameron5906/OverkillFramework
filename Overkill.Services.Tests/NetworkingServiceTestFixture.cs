using Microsoft.Extensions.Configuration;
using Moq;
using Overkill.Proxies.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Overkill.Services.Tests
{
    public class NetworkingServiceTestFixture
    {
        Mock<IProcessProxy> processProxy;
        Mock<IConfiguration> config;
        NetworkingService networkingService;

        public NetworkingServiceTestFixture()
        {
            processProxy = new Mock<IProcessProxy>();
            config = new Mock<IConfiguration>();
            networkingService = new NetworkingService(processProxy.Object);
        }
    }
}
