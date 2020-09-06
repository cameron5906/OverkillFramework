using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Overkill.Common.Enums;
using Overkill.Core;
using Overkill.Core.Connections;
using Overkill.Core.Interfaces;
using Overkill.Proxies;
using Overkill.Proxies.Interfaces;
using Overkill.PubSub;
using Overkill.PubSub.Interfaces;
using Overkill.Services;
using Overkill.Services.Interfaces.Services;
using Overkill.Services.Services;
using Overkill.Util.Helpers;
using Overkill.Websockets;
using Overkill.Websockets.Interfaces;
using System;
using System.IO;
using System.Reflection;

namespace Overkill
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Overkill is initializing");
            OverkillConfiguration config = new OverkillConfiguration();

            AppDomain.CurrentDomain.AssemblyResolve += (sender, evt) =>
            {
                var assemblyFile = evt.Name.Contains(",") ? evt.Name.Substring(0, evt.Name.IndexOf(",")) : evt.Name;

                assemblyFile += ".dll";

                var absoluteFolder = new FileInfo((new Uri(Assembly.GetExecutingAssembly().CodeBase)).LocalPath).Directory.FullName;
                var targetPath = Path.Combine(absoluteFolder, assemblyFile);

                try
                {
                    return Assembly.LoadFile(targetPath);
                } catch(Exception)
                {
                    return null;
                }
            };

            var host = new HostBuilder()
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder
                    .AddJsonFile("configuration.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                })
                .ConfigureServices((hostContext, services) =>
                {
                    Console.WriteLine("Registering services");
                    hostContext.Configuration.Bind(config);

                    Boot.SetupConfiguration(config);
                    Boot.LoadVehicleDriverWithDependencies(services);
                    Boot.LoadPluginsWithDependencies(services);

                    //Core
                    services.AddSingleton<IOverkillConfiguration>(_ => new OverkillConfiguration(config));
                    services.AddSingleton<IInputService, InputService>();
                    services.AddSingleton<IPubSubService, PubSubService>();
                    services.AddSingleton<IVideoTransmissionService, FFmpegVideoTransmissionService>();

                    //Positioning - optional, configurable
                    if (config.Positioning.Enabled)
                    {
                        switch (config.Positioning.Type)
                        {
                            case PositioningSystem.QuectelCM:
                                services.AddSingleton<IPositioningService, QuectelModemPositioningService>();
                                break;
                        }
                    }

                    //Connection protocol - configurable
                    switch(config.VehicleConnection.Type)
                    {
                        case CommunicationProtocol.TCP:
                            services.AddSingleton<IConnectionInterface, TcpConnectionInterface>();
                            break;
                        case CommunicationProtocol.GPIO:
                            services.AddSingleton<IConnectionInterface, GpioConnectionInterface>();
                            break;
                    }

                    //Proxies
                    services.AddSingleton<IHttpProxy, HttpProxy>();
                    services.AddSingleton<IProcessProxy, ProcessProxy>();
                    services.AddSingleton<IFilesystemProxy, FilesystemProxy>();
                    services.AddSingleton<ISoundPlayerProxy, SoundPlayerProxy>();
                    services.AddSingleton<IThreadProxy, ThreadProxy>();
                    services.AddSingleton<ISerialProxy, SerialProxy>();

                    //Services
                    services.AddSingleton<IAudioService, AudioService>();
                    services.AddSingleton<INetworkingService, NetworkingService>();
                    services.AddSingleton<ILoggingService, LoggingService>();

                    //Audio
                    services.AddSingleton<ISoundOut, WaveOut>(x => new WaveOut() { Device = WaveOutDevice.DefaultDevice });

                    //Websockets
                    services.AddSingleton<IWebsocketService, WebsocketService>();


                    Console.WriteLine("Done registering services");
                })
                .UseConsoleLifetime()
                .Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                //Do the actual bootup and run/initialize things
                Boot.SetupServiceProvider(services);
                Boot.LoadTopics();
                Boot.LoadConfiguredServices();
                Boot.Finish();

                Console.ReadKey(true);
            }
        }
    }
}
