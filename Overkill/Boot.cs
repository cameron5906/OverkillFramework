using Microsoft.Extensions.DependencyInjection;
using Overkill.Common.Enums;
using Overkill.Common.Exceptions;
using Overkill.Core.Connections.Initialization;
using Overkill.Core.Interfaces;
using Overkill.PubSub.Interfaces;
using Overkill.Services.Interfaces.Services;
using Overkill.Websockets.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Overkill
{
    public static class Boot
    {
        private static IOverkillConfiguration config;
        private static IServiceProvider serviceProvider;

        /// <summary>
        /// Assigns the service provider and configuration object that will be used during boot
        /// </summary>
        /// <param name="_serviceProvider">A service provider scope for dependency injection</param>
        /// <param name="_config">An Overkill configuration object loaded from disk</param>
        public static void SetupConfiguration(IOverkillConfiguration _config)
        {
            config = _config;
            Console.WriteLine("Apply configuration...");
        }

        public static void SetupServiceProvider(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider;
            Console.WriteLine("Apply services...");
        }

        /// <summary>
        /// Loads the vehicle driver specified in the configuration or throws an exception if one is not found
        /// </summary>
        /// <param name="services"></param>
        public static void LoadVehicleDriverWithDependencies(IServiceCollection services)
        {
            Console.WriteLine($"Loading vehicle driver: {config.System.Module}...");

            try
            {
                var assemblyPath = Path.GetFullPath($"Vehicle.{config.System.Module}.dll");
                services
                    .ForInterfacesMatching("^IVehicle$")
                    .OfAssemblies(Assembly.LoadFile(assemblyPath))
                    .AddSingletons();
            } catch(Exception ex)
            {
                throw new BootException("Failed to load vehicle driver", ex);
            }
        }

        /// <summary>
        /// Loads plugins that are specified in the configuration file
        /// </summary>
        public static void LoadPluginsWithDependencies(IServiceCollection services)
        {
            Console.WriteLine("Discovering plugins...");

            var pluginAssemblies = config.System.Plugins.Select(name => $"Plugin.{name}.dll");

            //Ensure the files are there before attempting to load them. If one is missing, throw a boot exception
            if(pluginAssemblies.Any(fileName => !File.Exists(fileName)))
            {
                throw new BootException("Not all plugins specified in the configuration file were found.");
            }

            pluginAssemblies
                .Select(fileName => Assembly.LoadFile(Path.GetFullPath(fileName)))
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsInterface && typeof(IPlugin).IsAssignableFrom(type))
                .ToList()
                .ForEach(type =>
                {
                    Console.WriteLine($"Adding plugin: {type.Name}");
                    services.Add(new ServiceDescriptor(typeof(IPlugin), type, ServiceLifetime.Singleton));
                });
        }

        /// <summary>
        /// Asks the PubSub service to locate all IPubSubTopic instances in the loaded assemblies
        /// </summary>
        public static void LoadTopics()
        {
            Console.WriteLine("Discovering PubSub topics...");

            try
            {
                serviceProvider.GetRequiredService<IPubSubService>().DiscoverTopics();
            } catch(Exception ex)
            {
                throw new BootException("Failed to load all topics", ex);
            }
        }

        /// <summary>
        /// Load optional/configurable services that align with configuration file
        /// </summary>
        public static void LoadConfiguredServices()
        {
            if (config.Positioning.Enabled)
            {
                Console.WriteLine($"Starting {config.Positioning.Type} positioning service...");
                try
                {
                    serviceProvider.GetRequiredService<IPositioningService>().Start();
                } catch(Exception ex)
                {
                    throw new BootException("Failed to start positioning serivce", ex);
                }
            }

            if (config.Streaming.Enabled)
            {
                Console.WriteLine("Starting video transmission service...");
                try
                {
                    serviceProvider.GetRequiredService<IVideoTransmissionService>().Start();
                } catch(Exception ex)
                {
                    throw new BootException("Failed to start video transmission service", ex);
                }
            }
        }

        /// <summary>
        /// Finish the boot process by connecting to online services and starting the vehicle driver
        /// </summary>
        public static void Finish()
        {
            Console.WriteLine(serviceProvider.GetRequiredService<IPlugin>());
            Console.WriteLine("Initializing plugins...");
            serviceProvider.GetServices<IPlugin>().ToList().ForEach(plugin => {
                try
                {
                    Console.WriteLine($"Initializing plugin: {plugin.GetType().Name}");
                    plugin.Initialize();
                }
                catch (Exception ex)
                {
                    throw new BootException($"Failed to initialize plugin: {plugin.GetType().Name}", ex);
                }
            });

            Console.WriteLine("Connecting to online services...");
            try
            {
                serviceProvider.GetRequiredService<IWebsocketService>().Start();
            } catch(Exception ex)
            {
                throw new BootException("Failed to connect to online services", ex);
            }

            Console.WriteLine("Establishing link with vehicle...");
            try
            {
                var localInterfaceAddress = serviceProvider.GetRequiredService<INetworkingService>().GetLocalInterfaceAddress(config.VehicleConnection.Interface);
                var connectionInterface = serviceProvider.GetRequiredService<IConnectionInterface>();
                
                switch(config.VehicleConnection.Type)
                {
                    case CommunicationProtocol.TCP:
                        connectionInterface.Initialize(new TcpInitialization()
                        {
                            Host = config.VehicleConnection.Host,
                            Port = config.VehicleConnection.Port,
                            LocalEndpoint = new IPEndPoint(IPAddress.Parse(localInterfaceAddress), 0)
                        });
                        break;
                    case CommunicationProtocol.GPIO:
                        throw new BootException("The GPIO connection interface is not yet supported");
                }

                connectionInterface.Connect();

                Console.WriteLine("Link established");
                serviceProvider.GetRequiredService<IVehicle>().Initialize();
            } catch(Exception ex)
            {
                throw new BootException("Failed to connect to the vehicle", ex);
            }
        }
    }
}
