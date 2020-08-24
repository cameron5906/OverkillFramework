using Microsoft.Extensions.DependencyInjection;
using Overkill.Core.Interfaces;
using Overkill.PubSub.Interfaces;
using Overkill.Websockets.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Overkill
{
    public class Boot
    {
        private static IOverkillConfiguration config;
        private static IServiceProvider serviceProvider;

        public static void Setup(IServiceProvider _serviceProvider, IOverkillConfiguration _config)
        {
            config = _config;
            serviceProvider = _serviceProvider;
            Console.WriteLine("Booting...");
        }

        public static void LoadVehicleDriver(IServiceCollection services)
        {
            Console.WriteLine($"Loading vehicle driver: {config.System.Module}...");
            var assemblyPath = Path.GetFullPath($"Vehicle.{config.System.Module}.dll");
            services
                .ForInterfacesMatching("^IVehicle$")
                .OfAssemblies(Assembly.LoadFile(assemblyPath))
                .AddSingletons();
        }

        public static void LoadPlugins()
        {
            Console.WriteLine("Discovering plugins...");
            var pluginAssemblies = Directory.GetFiles(Environment.CurrentDirectory, "Plugin.*.dll");
            pluginAssemblies
                .Select(x => Assembly.LoadFile(x))
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsInterface && typeof(IPlugin).IsAssignableFrom(x))
                .Select(x => (IPlugin)Activator.CreateInstance(x, new[] { serviceProvider }))
                .ToList()
                .ForEach(plugin =>
                {
                    Console.WriteLine($"Initializing plugin: {plugin.GetType().Name}");
                    plugin.Initialize();
                });
        }

        public static void LoadTopics()
        {
            Console.WriteLine("Discovering PubSub topics...");
            serviceProvider.GetRequiredService<IPubSubService>().DiscoverTopics();
        }

        public static void LoadConfiguredServices()
        {
            if (config.Positioning.Enabled)
            {
                Console.WriteLine($"Starting {config.Positioning.Type} positioning service...");
                serviceProvider.GetRequiredService<IPositioningService>().Start();
            }

            if (config.Streaming.Enabled)
            {
                Console.WriteLine("Starting video transmission service...");
                serviceProvider.GetRequiredService<IVideoTransmissionService>().Start();
            }
        }

        public static void Finish()
        {
            Console.WriteLine("Connecting to online services...");
            serviceProvider.GetRequiredService<IWebsocketService>().Start();

            Console.WriteLine("Starting vehicle driver...");
            serviceProvider.GetRequiredService<IVehicle>().Initialize();
        }
    }
}
